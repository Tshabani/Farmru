import { Component, Injector, ChangeDetectorRef, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import {
  FacilityDto,
  FacilityServiceProxy,
  NodeDto,
  NodeDtoPagedResultDto,
  NodeServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { EditNodeComponent } from './edit-node/edit-node.component';
import { CreateNodeComponent } from './create-node/create-node.component';
import { ActivatedRoute, Router } from '@angular/router';

class PagedNodesRequestDto extends PagedRequestDto {
  keyword: string;
  facilityId?: string;
  deviceStatus?: number;
  healthStatus?: number;
  onlineOnly?: boolean;
  offlineOnly?: boolean;
}

@Component({
  templateUrl: './node.component.html',
  animations: [appModuleAnimation()],
})
export class NodeComponent extends PagedListingComponentBase<NodeDto> implements OnInit {
  nodes: NodeDto[] = [];
  keyword = '';
  facilities: FacilityDto[] = [];
  selectedFacilityId: string | undefined;
  selectedDeviceStatus: number | undefined;
  selectedHealthStatus: number | undefined;
  onlineFilter: 'all' | 'online' | 'offline' = 'all';

  deviceStatusOptions = [
    { value: 0, label: 'Active' },
    { value: 1, label: 'Inactive' },
    { value: 2, label: 'Maintenance' },
    { value: 3, label: 'Decommissioned' },
  ];

  healthStatusOptions = [
    { value: 0, label: 'Unknown' },
    { value: 1, label: 'Healthy' },
    { value: 2, label: 'Warning' },
    { value: 3, label: 'Critical' },
  ];

  constructor(
    injector: Injector,
    private _nodesService: NodeServiceProxy,
    private _facilityService: FacilityServiceProxy,
    private _modalService: BsModalService,
    private router: Router,
    private route: ActivatedRoute,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {
    super.ngOnInit();
    this._facilityService.getAll(undefined, true, 0, 200).subscribe((r) => {
      this.facilities = r.items;
      this.cd.detectChanges();
    });
  }

  onRowClick(rowId: string): void {
    this.router.navigate(['details', rowId], { relativeTo: this.route });
  }

  viewTelemetry(node: NodeDto): void {
    this.router.navigate(['nodeData', node.id], { relativeTo: this.route });
  }

  healthLabel(node: NodeDto): string {
    return this.healthStatusOptions.find((h) => h.value === node.healthStatus)?.label ?? 'Unknown';
  }

  healthBadgeClass(node: NodeDto): string {
    switch (node.healthStatus) {
      case 1:
        return 'badge-success';
      case 2:
        return 'badge-warning';
      case 3:
        return 'badge-danger';
      default:
        return 'badge-secondary';
    }
  }

  list(request: PagedNodesRequestDto, pageNumber: number, finishedCallback: Function): void {
    request.keyword = this.keyword;
    request.facilityId = this.selectedFacilityId;
    request.deviceStatus = this.selectedDeviceStatus;
    request.healthStatus = this.selectedHealthStatus;
    request.onlineOnly = this.onlineFilter === 'online';
    request.offlineOnly = this.onlineFilter === 'offline';

    this._nodesService
      .getAllFiltered(
        request.keyword,
        request.facilityId,
        request.deviceStatus,
        request.healthStatus,
        request.onlineOnly,
        request.offlineOnly,
        undefined,
        request.skipCount,
        request.maxResultCount
      )
      .pipe(finalize(() => finishedCallback()))
      .subscribe((result: NodeDtoPagedResultDto) => {
        this.nodes = result.items;
        this.showPaging(result, pageNumber);
        this.cd.detectChanges();
      });
  }

  clearFilters(): void {
    this.keyword = '';
    this.selectedFacilityId = undefined;
    this.selectedDeviceStatus = undefined;
    this.selectedHealthStatus = undefined;
    this.onlineFilter = 'all';
    this.getDataPage(1);
  }

  delete(node: NodeDto): void {
    abp.message.confirm(
      this.l('NodeDeleteWarningMessage', node.serialNumber),
      undefined,
      (result: boolean) => {
        if (result) {
          this._nodesService
            .delete(node.id)
            .pipe(
              finalize(() => {
                abp.notify.success(this.l('SuccessfullyDeleted'));
                this.refresh();
              })
            )
            .subscribe(() => {});
        }
      }
    );
  }

  createNode(): void {
    this.showCreateOrEditNodeDialog();
  }

  editNode(node: NodeDto): void {
    this.showCreateOrEditNodeDialog(node.id);
  }

  showCreateOrEditNodeDialog(id?: string): void {
    let createOrEditNodeDialog: BsModalRef;
    if (!id) {
      createOrEditNodeDialog = this._modalService.show(CreateNodeComponent, { class: 'modal-lg' });
    } else {
      createOrEditNodeDialog = this._modalService.show(EditNodeComponent, {
        class: 'modal-lg',
        initialState: { id },
      });
    }

    createOrEditNodeDialog.content.onSave.subscribe(() => this.refresh());
  }
}

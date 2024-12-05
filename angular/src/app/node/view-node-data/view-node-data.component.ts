import { ChangeDetectorRef, Component, Injector } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { NodeDataDto, NodeDataDtoPagedResultDto, NodeDataServiceProxy } from '@shared/service-proxies/service-proxies';
import moment from 'moment';
import { Moment } from 'moment';
import { BsModalService } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs';

class PagedNodeDataRequestDto extends PagedRequestDto {
  predefinedPeriod:string;
  startDate:Moment | undefined;
  endDate:Moment | undefined;
  keyword: string;
}

@Component({
  templateUrl: './view-node-data.component.html',
  animations: [appModuleAnimation()]
})

export class ViewNodeDataComponent extends PagedListingComponentBase<NodeDataDto> {
  nodeId: string;
  nodeData: NodeDataDto[] = [];
  keyword:string = '';
  predefinedPeriod:string = '';
  _startDate:Moment | undefined = undefined;
  _endDate:Moment | undefined = undefined;
  advancedFiltersVisible = false;

  constructor(
    injector: Injector,
    private _nodesService: NodeDataServiceProxy,
    private route: ActivatedRoute,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
    this.nodeId = this.route.snapshot.paramMap.get('id');
  }

  set startDate(value: string | null) {
    this._startDate = value ? moment(value, 'YYYY-MM-DD') : undefined;
  }

  set endDate(value: string | null) {
    this._endDate = value ? moment(value, 'YYYY-MM-DD') : undefined;
  }

  clearFilters(): void {
    this.keyword = '';
    this.predefinedPeriod = '';
    this.startDate = undefined;
    this.endDate = undefined;
    this.getDataPage(1);
  }

  list(
    request: PagedNodeDataRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    request.predefinedPeriod = this.predefinedPeriod;
    request.startDate = this._startDate;
    request.endDate = this._endDate;

    this._nodesService.getNodeDataByNodeId(this.nodeId,request.startDate,request.endDate,request.predefinedPeriod, request.skipCount, request.maxResultCount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: NodeDataDtoPagedResultDto) => {
        this.nodeData = result.items;
        this.showPaging(result, pageNumber);
        this.cd.detectChanges();
      });
  }

  delete(node: NodeDataDto): void {
    abp.message.confirm(
      this.l('NodeDeleteWarningMessage', node.id),
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
}

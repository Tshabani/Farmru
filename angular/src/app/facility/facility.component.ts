import { ChangeDetectorRef, Component, Injector } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedRequestDto, PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { FacilityDto, FacilityServiceProxy, FacilityDtoPagedResultDto } from '@shared/service-proxies/service-proxies';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs';
import { CreateFacilityComponent } from './create-facility/create-facility.component';
import { EditFacilityComponent } from './edit-facility/edit-facility.component';

class PagedFacilitysRequestDto extends PagedRequestDto {
  keyword: string;
}

@Component({
  templateUrl: './facility.component.html',  
  animations: [appModuleAnimation()]
})
export class FacilityComponent extends PagedListingComponentBase<FacilityDto> {
  facilities: FacilityDto[] = [];
  keyword = '';

  constructor(
    injector: Injector,
    private _facilitysService: FacilityServiceProxy,
    private _modalService: BsModalService,
    private router: Router,
    private route: ActivatedRoute,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  onRowClick(rowId: string): void {
    this.router.navigate(['facilityData', rowId], { relativeTo: this.route });
  }

  list(
    request: PagedFacilitysRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;

    this._facilitysService
      .getAll(request.keyword, true, request.skipCount, request.maxResultCount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: FacilityDtoPagedResultDto) => {
        this.facilities = result.items;
        this.showPaging(result, pageNumber);
        this.cd.detectChanges();
      });
  }

  delete(facility: FacilityDto): void {
    abp.message.confirm(
      this.l('FacilityDeleteWarningMessage', facility.id),
      undefined,
      (result: boolean) => {
        if (result) {
          this._facilitysService
            .delete(facility.id)
            .pipe(
              finalize(() => {
                abp.notify.success(this.l('SuccessfullyDeleted'));
                this.refresh();
              })
            )
            .subscribe(() => { });
        }
      }
    );
  }

  createFacility(): void {
    this.showCreateOrEditFacilityDialog();
  }

  editFacility(facility: FacilityDto): void {
    this.showCreateOrEditFacilityDialog(facility.id);
  }

  showCreateOrEditFacilityDialog(id?: string): void {
    let createOrEditFacilityDialog: BsModalRef;
    if (!id) {
      createOrEditFacilityDialog = this._modalService.show(
        CreateFacilityComponent,
        {
          class: 'modal-lg',
        }
      );
    } else {
      createOrEditFacilityDialog = this._modalService.show(
        EditFacilityComponent,
        {
          class: 'modal-lg',
          initialState: {
            id: id,
          },
        }
      );
    }

    createOrEditFacilityDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
}

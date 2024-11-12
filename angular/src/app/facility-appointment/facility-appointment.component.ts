import { ChangeDetectorRef, Component, Injector } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedRequestDto, PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { FacilityAppointmentDto, FacilityAppointmentServiceProxy, FacilityAppointmentDtoPagedResultDto } from '@shared/service-proxies/service-proxies';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs';
import { CreateFacilityAppointmentComponent } from './create-facility-appointment/create-facility-appointment.component';
import { EditFacilityAppointmentComponent } from './edit-facility-appointment/edit-facility-appointment.component';

class PagedFacilityAppointmentsRequestDto extends PagedRequestDto {
  keyword: string;
}

@Component({
  templateUrl: './facility-appointment.component.html',  
  animations: [appModuleAnimation()]
})
export class FacilityAppointmentComponent extends PagedListingComponentBase<FacilityAppointmentDto> {
  facilityAppointments: FacilityAppointmentDto[] = [];
  keyword = '';

  constructor(
    injector: Injector,
    private _facilityAppointmentsService: FacilityAppointmentServiceProxy,
    private _modalService: BsModalService,
    private router: Router,
    private route: ActivatedRoute,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  onRowClick(rowId: string): void {
    this.router.navigate(['facilityAppointmentData', rowId], { relativeTo: this.route });
  }

  list(
    request: PagedFacilityAppointmentsRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;

    this._facilityAppointmentsService
      .getAll(request.keyword, true, request.skipCount, request.maxResultCount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: FacilityAppointmentDtoPagedResultDto) => {
        this.facilityAppointments = result.items;
        this.showPaging(result, pageNumber);
        this.cd.detectChanges();
      });
  }

  delete(facilityAppointment: FacilityAppointmentDto): void {
    abp.message.confirm(
      this.l('FacilityAppointmentDeleteWarningMessage', facilityAppointment.id),
      undefined,
      (result: boolean) => {
        if (result) {
          this._facilityAppointmentsService
            .delete(facilityAppointment.id)
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

  createFacilityAppointment(): void {
    this.showCreateOrEditFacilityAppointmentDialog();
  }

  editFacilityAppointment(facilityAppointment: FacilityAppointmentDto): void {
    this.showCreateOrEditFacilityAppointmentDialog(facilityAppointment.id);
  }

  showCreateOrEditFacilityAppointmentDialog(id?: string): void {
    let createOrEditFacilityAppointmentDialog: BsModalRef;
    if (!id) {
      createOrEditFacilityAppointmentDialog = this._modalService.show(
        CreateFacilityAppointmentComponent,
        {
          class: 'modal-lg',
        }
      );
    } else {
      createOrEditFacilityAppointmentDialog = this._modalService.show(
        EditFacilityAppointmentComponent,
        {
          class: 'modal-lg',
          initialState: {
            id: id,
          },
        }
      );
    }

    createOrEditFacilityAppointmentDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
}

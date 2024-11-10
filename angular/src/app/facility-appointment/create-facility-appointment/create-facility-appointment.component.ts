import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { FacilityAppointmentDto, FacilityAppointmentServiceProxy, FacilityDto } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './create-facility-appointment.component.html',
})
export class CreateFacilityAppointmentComponent  extends AppComponentBase
implements OnInit {

  saving = false;
  createFacility = new FacilityAppointmentDto();

  onSave = output<EventEmitter<any>>()

  constructor(
    injector: Injector,
    private _createFacilityService: FacilityAppointmentServiceProxy,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.cd.detectChanges();
  }

  save(): void {
    this.saving = true;

    const createFacility = new FacilityAppointmentDto();
    createFacility.init(this.createFacility);

    this._createFacilityService
      .create(createFacility)
      .subscribe(
        () => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit(null);
        },
        () => {
          this.saving = false;
          this.cd.detectChanges();
        }
    );
  }
}

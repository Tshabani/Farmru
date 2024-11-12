import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { FacilityAppointmentDto, FacilityAppointmentServiceProxy, FacilityDto, FacilityServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './edit-facility-appointment.component.html',
})
export class EditFacilityAppointmentComponent extends AppComponentBase
implements OnInit {
  id: string;
  saving = false;
  facilityAppointment = new FacilityAppointmentDto();  

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _facilityAppointmentService: FacilityAppointmentServiceProxy,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._facilityAppointmentService.get(this.id).subscribe((result) => {
      this.facilityAppointment = result; 
      this.cd.detectChanges();  
    });
  }
    
  save(): void {
    this.saving = true;  
    const facilityAppointment = new FacilityAppointmentDto();
    facilityAppointment.init(this.facilityAppointment);

    this._facilityAppointmentService
      .update(facilityAppointment)
      .subscribe(
        () => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();   
          this.onSave.emit();         
        },
        () => {
          this.saving = false;
          this.cd.detectChanges();
        }
      );
  }
}

import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { FacilitiesDto, FacilityAppointmentDto, FacilityAppointmentServiceProxy, FacilityDto, FacilityServiceProxy, GuidNullableEntityWithDisplayNameDto, PeopleDto, PersonServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './edit-facility-appointment.component.html',
})
export class EditFacilityAppointmentComponent extends AppComponentBase
implements OnInit {
  id: string;
  saving = false;
  appointment = new FacilityAppointmentDto();
  people: PeopleDto[] = []; 
  facilities: FacilitiesDto[] = []; 
  selectedAppointedUserId: string | null = null;
  selectedFacilityId: string | null = null;

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _facilityAppointmentService: FacilityAppointmentServiceProxy,     
    private _personService: PersonServiceProxy,
    private _facilitiesService: FacilityServiceProxy,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._facilityAppointmentService.get(this.id).subscribe((result) => {
      this.appointment = result; 
      this.selectedAppointedUserId = result.appointedUser ? result.appointedUser.id : null;
      this.selectedFacilityId = result.facility ? result.facility.id : null;      
      this._personService.getListOfPeople().subscribe((result) => {
        this.people = result;         
        this._facilitiesService.getListOfFacilities().subscribe((result) => {
          this.facilities = result;           
          this.cd.detectChanges();  
        }); 
      });   
    });
  }

  onAppointedUserChange(selectedId: string): void {
    const selectedPerson = this.people.find(person => person.id === selectedId);
  
    if (!this.appointment.appointedUser) {
      this.appointment.appointedUser = new GuidNullableEntityWithDisplayNameDto;
    }
  
    if (selectedPerson) {
      this.appointment.appointedUser.id = selectedPerson.id;
      this.appointment.appointedUser.displayText = selectedPerson.fullName;
    } else {
      this.appointment.appointedUser.id = null;
      this.appointment.appointedUser.displayText = null;
    }
  }
  
  onFacilityChange(selectedId: string): void {
    const selectedPerson = this.people.find(person => person.id === selectedId);
  
    if (!this.appointment.facility) {
      this.appointment.facility = new GuidNullableEntityWithDisplayNameDto;
    }
  
    if (selectedPerson) {
      this.appointment.facility.id = selectedPerson.id;
      this.appointment.facility.displayText = selectedPerson.fullName;
    } else {
      this.appointment.facility.id = null;
      this.appointment.facility.displayText = null;
    }
  }

  trackById(index: number, item: any): any {
    return item.id;
  }

    
  save(): void {
    this.saving = true;  
    const facilityAppointment = new FacilityAppointmentDto();
    facilityAppointment.init(this.appointment);

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

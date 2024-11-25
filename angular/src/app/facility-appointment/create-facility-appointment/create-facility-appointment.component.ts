import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateFacilityDto, FacilitiesDto, FacilityAppointmentDto, FacilityAppointmentServiceProxy, FacilityDto, FacilityServiceProxy, GuidNullableEntityWithDisplayNameDto, PeopleDto, PersonServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './create-facility-appointment.component.html',
})
export class CreateFacilityAppointmentComponent  extends AppComponentBase
implements OnInit {

  saving = false;
  appointment = new FacilityAppointmentDto();
  people: PeopleDto[] = []; 
  facilities: FacilitiesDto[] = []; 
  selectedAppointedUserId: string | null = null;
  selectedFacilityId: string | null = null;

  onSave = output<EventEmitter<any>>()

  constructor(
    injector: Injector,
    private _CreateAppointmentService: FacilityAppointmentServiceProxy,
    private _personService: PersonServiceProxy,    
    private _facilitiesService: FacilityServiceProxy,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._personService.getListOfPeople().subscribe((result) => {
      this.people = result; 
      this._facilitiesService.getListOfFacilities().subscribe((result) => {
        this.facilities = result;           
        this.cd.detectChanges();  
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
    const selectedFacility = this.facilities.find(facility => facility.id === selectedId);
    
    if (!this.appointment.facility) {
      this.appointment.facility = new GuidNullableEntityWithDisplayNameDto;
    }
  
    if (selectedFacility) {
      this.appointment.facility.id = selectedFacility.id;
      this.appointment.facility.displayText = selectedFacility.name;
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

    const CreateAppointment = new FacilityAppointmentDto();
    CreateAppointment.init(this.appointment);

    this._CreateAppointmentService
      .create(CreateAppointment)
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

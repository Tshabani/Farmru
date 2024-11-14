import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, output, Output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateFacilityDto, FacilityServiceProxy, GuidNullableEntityWithDisplayNameDto, PeopleDto, PersonDto, PersonServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './create-facility.component.html',
})
export class CreateFacilityComponent extends AppComponentBase
implements OnInit {

  saving = false;
  facility = new CreateFacilityDto(); 
  people: PeopleDto[] = []; 
  selectedOwnerOrganisationId: string | null = null;
  selectedprimaryContactId: string | null = null;

  onSave = output<EventEmitter<any>>()

  constructor(
    injector: Injector,
    private _facilityService: FacilityServiceProxy,
    private _personService: PersonServiceProxy,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._personService.getListOfPeople().subscribe((result) => {
      this.people = result; 
      this.cd.detectChanges();  
    });
  }

  onOwnerOrganisationChange(selectedId: string): void {
    const selectedPerson = this.people.find(person => person.id === selectedId);
  
    if (!this.facility.ownerOrganisation) {
      this.facility.ownerOrganisation = new GuidNullableEntityWithDisplayNameDto;
    }
  
    if (selectedPerson) {
      this.facility.ownerOrganisation.id = selectedPerson.id;
      this.facility.ownerOrganisation.displayText = selectedPerson.fullName;
    } else {
      this.facility.ownerOrganisation.id = null;
      this.facility.ownerOrganisation.displayText = null;
    }
  }
  
  onPrimaryContactChange(selectedId: string): void {
    const selectedPerson = this.people.find(person => person.id === selectedId);
  
    if (!this.facility.primaryContact) {
      this.facility.primaryContact = new GuidNullableEntityWithDisplayNameDto;
    }
  
    if (selectedPerson) {
      this.facility.primaryContact.id = selectedPerson.id;
      this.facility.primaryContact.displayText = selectedPerson.fullName;
    } else {
      this.facility.primaryContact.id = null;
      this.facility.primaryContact.displayText = null;
    }
  }

  trackById(index: number, item: any): any {
    return item.id;
  }
    
  save(): void {
    this.saving = true;     
    debugger;  
    const facility = new CreateFacilityDto();
    facility.init(this.facility);

    this._facilityService
      .create(this.facility)
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

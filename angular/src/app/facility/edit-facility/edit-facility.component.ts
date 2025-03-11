import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { FacilityDto, FacilityServiceProxy, GuidNullableEntityWithDisplayNameDto, OrganisationsDto, OrganisationServiceProxy, PeopleDto, PersonDto, PersonServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { SafeDisplayPipe } from '@shared/pipes/safe-display.pipe'

@Component({
  templateUrl: './edit-facility.component.html',
})
export class EditFacilityComponent extends AppComponentBase
implements OnInit {
  id: string;
  saving = false;
  facility = new FacilityDto();  
  people: PeopleDto[] = []; 
  organisations: OrganisationsDto[] = []; 
  selectedOwnerOrganisationId: string | null = null;
  selectedprimaryContactId: string | null = null;

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _facilityService: FacilityServiceProxy,    
    private _personService: PersonServiceProxy,
    private _organisationsService: OrganisationServiceProxy,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._facilityService.get(this.id).subscribe((result) => {
      this.facility = result;
      this.selectedOwnerOrganisationId = result.ownerOrganisation ? result.ownerOrganisation.id : null;
      this.selectedprimaryContactId = result.primaryContact ? result.primaryContact.id : null;      
      this._personService.getListOfPeople().subscribe((result) => {
        this.people = result;
        this._organisationsService.getListOfOrganisations().subscribe((result) => {
          this.organisations = result;           
          this.cd.detectChanges();  
        });   
      });  
    });
  }

  onOwnerOrganisationChange(selectedId: string): void {
    const selectedPerson = this.organisations.find(org => org.id === selectedId);
  
    if (!this.facility.ownerOrganisation) {
      this.facility.ownerOrganisation = new GuidNullableEntityWithDisplayNameDto;
    }
  
    if (selectedPerson) {
      this.facility.ownerOrganisation.id = selectedPerson.id;
      this.facility.ownerOrganisation.displayText = selectedPerson.name;
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
    const facility = new FacilityDto();
    facility.init(this.facility);

    this._facilityService
      .update(facility)
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

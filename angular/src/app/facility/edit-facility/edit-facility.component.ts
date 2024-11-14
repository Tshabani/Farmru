import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { FacilityDto, FacilityServiceProxy, GuidNullableEntityWithDisplayNameDto, PeopleDto, PersonDto, PersonServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './edit-facility.component.html',
})
export class EditFacilityComponent extends AppComponentBase
implements OnInit {
  id: string;
  saving = false;
  facility = new FacilityDto();  
  people: GuidNullableEntityWithDisplayNameDto[] = []; 
  selectedOwnerOrganisationId: string | null = null;
  selectedprimaryContactId: string | null = null;

  @Output() onSave = new EventEmitter<any>();

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
    this._facilityService.get(this.id).subscribe((result) => {
      this.facility = result; 
      this._personService.getAll().subscribe((result) => {
        this.people = result.list;
        debugger; 
        this.cd.detectChanges();  
      });  
    });
  }

  onOwnerOrganisationChange(selectedId: string): void {
    const selectedPerson = this.people.find(person => person.id === selectedId);

    if (selectedPerson) {
      this.facility.ownerOrganisation.id = selectedPerson.id;
      this.facility.ownerOrganisation.displayText = selectedPerson.displayText;
    } else {
      this.facility.ownerOrganisation.id = null;
      this.facility.ownerOrganisation.displayText = null;
    }
  }

  onprimaryContactChange(selectedId: string): void {
    const selectedPerson = this.people.find(person => person.id === selectedId);

    if (selectedPerson) {
      this.facility.primaryContact.id = selectedPerson.id;
      this.facility.primaryContact.displayText = selectedPerson.displayText;
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

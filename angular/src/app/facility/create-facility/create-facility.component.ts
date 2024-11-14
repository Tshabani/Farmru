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
  people: GuidNullableEntityWithDisplayNameDto[] = []; 

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
    this._personService.getAll().subscribe((result) => {
      this.people = result.list; 
      this.cd.detectChanges();  
    });
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

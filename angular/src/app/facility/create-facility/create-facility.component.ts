import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { FacilityDto, FacilityServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './create-facility.component.html',
})
export class CreateFacilityComponent  extends AppComponentBase
implements OnInit {
  id: string;
  saving = false;
  facility = new FacilityDto();  

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _facilityService: FacilityServiceProxy,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._facilityService.get(this.id).subscribe((result) => {
      this.facility = result; 
      this.cd.detectChanges();  
    });
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

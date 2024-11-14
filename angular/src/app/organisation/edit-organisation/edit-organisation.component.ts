import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { OrganisationDto, OrganisationServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './edit-organisation.component.html',
})
export class EditOrganisationComponent  extends AppComponentBase
implements OnInit {
  id: string;
  saving = false;
  organisation = new OrganisationDto();  

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _organisationService: OrganisationServiceProxy,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._organisationService.get(this.id).subscribe((result) => {
      this.organisation = result; 
      this.cd.detectChanges();  
    });
  }
    
  save(): void {
    this.saving = true;  
    const organisation = new OrganisationDto();
    organisation.init(this.organisation);

    this._organisationService
      .update(organisation)
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

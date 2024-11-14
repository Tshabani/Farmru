import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { OrganisationDto, OrganisationServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './create-organisation.component.html',
})
export class CreateOrganisationComponent extends AppComponentBase
implements OnInit {

  saving = false;
  organisation = new OrganisationDto();

  onSave = output<EventEmitter<any>>()

  constructor(
    injector: Injector,
    private _organisationService: OrganisationServiceProxy,
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

    const organisation = new OrganisationDto();
    organisation.init(this.organisation);

    this._organisationService
      .create(organisation)
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

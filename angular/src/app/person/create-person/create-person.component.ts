import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CreatePersonAccountDto, PersonDto, PersonServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './create-person.component.html',
})

export class CreatePersonComponent extends AppComponentBase
implements OnInit {

  saving = false;
  person = new PersonDto();

  onSave = output<EventEmitter<any>>()

  constructor(
    injector: Injector,
    private _personService: PersonServiceProxy,
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

    const person = new CreatePersonAccountDto();
    person.init(this.person);

    this._personService
      .create(person)
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

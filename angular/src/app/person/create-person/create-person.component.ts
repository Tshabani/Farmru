import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { getGenderOptions, getPersonTitles } from '@shared/helpers/userHealper';
import { CreatePersonDto, PersonDto, PersonServiceProxy } from '@shared/service-proxies/service-proxies';
import moment from 'moment';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './create-person.component.html',
})
export class CreatePersonComponent extends AppComponentBase
implements OnInit {

  saving = false;
  person = new PersonDto();
  personTitles = getPersonTitles();
  genderOptions = getGenderOptions();

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

  onDateOfBirthChange(event: any) {
    this.person.dateOfBirth = event ? moment(event) : null;
  }

  save(): void {
    this.saving = true;

    const person = new CreatePersonDto();
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

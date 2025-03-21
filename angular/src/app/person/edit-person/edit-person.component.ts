import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { getPersonTitles, getGenderOptions } from '@shared/helpers/userHealper';
import { PersonDto, PersonServiceProxy } from '@shared/service-proxies/service-proxies';
import moment from 'moment';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './edit-person.component.html',
})
export class EditPersonComponent  extends AppComponentBase
implements OnInit {
  id: string;
  saving = false;
  person = new PersonDto();  
  personTitles = getPersonTitles();
  genderOptions = getGenderOptions();

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _personService: PersonServiceProxy,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._personService.get(this.id).subscribe((result) => {
      this.person = result; 
      this.cd.detectChanges();  
    });
  }
 
  onDateOfBirthChange(event: any) {
    this.person.dateOfBirth = event ? moment(event) : null;
  }
    
  save(): void {
    this.saving = true;  
    const person = new PersonDto();
    person.init(this.person);

    this._personService
      .update(person)
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

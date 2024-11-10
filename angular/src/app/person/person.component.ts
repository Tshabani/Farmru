import { ChangeDetectorRef, Component, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedRequestDto, PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { PersonDto, PersonDtoPagedResultDto, PersonServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs';
import { CreatePersonComponent } from './create-person/create-person.component';
import { EditPersonComponent } from './edit-person/edit-person.component';

class PagedPersonsRequestDto extends PagedRequestDto {
  keyword: string;  
  isActive: boolean | null;
}

@Component({
  templateUrl: './person.component.html',
  animations: [appModuleAnimation()]
})
export class PersonComponent extends PagedListingComponentBase<PersonDto> {
  people: PersonDto[] = [];
  keyword = '';
  isActive: boolean | null;
  advancedFiltersVisible = false;

  constructor(
    injector: Injector,
    private _personService: PersonServiceProxy,
    private _modalService: BsModalService,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  createPerson(): void {
    this.showCreateOrEditPersonDialog();
  }

  editPerson(person: PersonDto): void {
    this.showCreateOrEditPersonDialog(person.id);
  }

  clearFilters(): void {
    this.keyword = '';
    this.isActive = undefined;
    this.getDataPage(1);
  }

  protected list(
    request: PagedPersonsRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    request.isActive = this.isActive;

    this._personService
      .getAll(
        request.keyword,
        request.isActive,
        request.skipCount,
        request.maxResultCount
      )
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: PersonDtoPagedResultDto) => {
        this.people = result.items;
        this.showPaging(result, pageNumber);
      });
  }

  protected delete(person: PersonDto): void {
    abp.message.confirm(
      this.l('PersonDeleteWarningMessage', person.fullName),
      undefined,
      (result: boolean) => {
        if (result) {
          this._personService.delete(person.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.refresh();
          });
        }
      }
    );
  }

  private showCreateOrEditPersonDialog(id?: string): void {
    let createOrEditPersonDialog: BsModalRef;
    if (!id) {
      createOrEditPersonDialog = this._modalService.show(
        CreatePersonComponent,
        {
          class: 'modal-lg',
        }
      );
    } else {
      createOrEditPersonDialog = this._modalService.show(
        EditPersonComponent,
        {
          class: 'modal-lg',
          initialState: {
            id: id,
          },
        }
      );
    }

    createOrEditPersonDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
}

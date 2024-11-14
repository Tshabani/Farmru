import { ChangeDetectorRef, Component, Injector } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedRequestDto, PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { PersonDto, PersonServiceProxy, PersonDtoPagedResultDto } from '@shared/service-proxies/service-proxies';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs';
import { CreatePersonComponent } from './create-person/create-person.component';
import { EditPersonComponent } from './edit-person/edit-person.component';

class PagedPersonsRequestDto extends PagedRequestDto {
  keyword: string;
}
@Component({
  templateUrl: './person.component.html',
  animations: [appModuleAnimation()]
})
export class PersonComponent extends PagedListingComponentBase<PersonDto> {
  people: PersonDto[] = [];
  keyword = '';

  constructor(
    injector: Injector,
    private _personsService: PersonServiceProxy,
    private _modalService: BsModalService,
    private router: Router,
    private route: ActivatedRoute,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  onRowClick(rowId: string): void {
    this.router.navigate(['personData', rowId], { relativeTo: this.route });
  }

  list(
    request: PagedPersonsRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;

    this._personsService
      .getAll(request.skipCount, request.maxResultCount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: PersonDtoPagedResultDto) => {
        this.people = result.items;
        this.showPaging(result, pageNumber);
        this.cd.detectChanges();
      });
  }

  delete(person: PersonDto): void {
    abp.message.confirm(
      this.l('PersonDeleteWarningMessage', person.firstName),
      undefined,
      (result: boolean) => {
        if (result) {
          this._personsService
            .delete(person.id)
            .pipe(
              finalize(() => {
                abp.notify.success(this.l('SuccessfullyDeleted'));
                this.refresh();
              })
            )
            .subscribe(() => { });
        }
      }
    );
  }

  createPerson(): void {
    this.showCreateOrEditPersonDialog();
  }

  editPerson(person: PersonDto): void {
    this.showCreateOrEditPersonDialog(person.id);
  }

  showCreateOrEditPersonDialog(id?: string): void {
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

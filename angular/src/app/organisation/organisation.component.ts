import { ChangeDetectorRef, Component, Injector } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { OrganisationDto, OrganisationServiceProxy, OrganisationDtoPagedResultDto } from '@shared/service-proxies/service-proxies';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs';
import { CreateOrganisationComponent } from './create-organisation/create-organisation.component';
import { EditOrganisationComponent } from './edit-organisation/edit-organisation.component';

class PagedOrganisationsRequestDto extends PagedRequestDto {
  keyword: string;
}
@Component({
  templateUrl: './organisation.component.html',
  animations: [appModuleAnimation()]
})
export class OrganisationComponent extends PagedListingComponentBase<OrganisationDto> {
  organisations: OrganisationDto[] = [];
  keyword = '';

  constructor(
    injector: Injector,
    private _organisationsService: OrganisationServiceProxy,
    private _modalService: BsModalService,
    private router: Router,
    private route: ActivatedRoute,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  onRowClick(rowId: string): void {
    this.router.navigate(['organisationData', rowId], { relativeTo: this.route });
  }

  list(
    request: PagedOrganisationsRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;

    this._organisationsService
      .getAll(request.skipCount, request.maxResultCount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: OrganisationDtoPagedResultDto) => {
        this.organisations = result.items;
        this.showPaging(result, pageNumber);
        this.cd.detectChanges();
      });
  }

  delete(organisation: OrganisationDto): void {
    abp.message.confirm(
      this.l('OrganisationDeleteWarningMessage', organisation.name),
      undefined,
      (result: boolean) => {
        if (result) {
          this._organisationsService
            .delete(organisation.id)
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

  createOrganisation(): void {
    this.showCreateOrEditOrganisationDialog();
  }

  editOrganisation(organisation: OrganisationDto): void {
    this.showCreateOrEditOrganisationDialog(organisation.id);
  }

  showCreateOrEditOrganisationDialog(id?: string): void {
    let createOrEditOrganisationDialog: BsModalRef;
    if (!id) {
      createOrEditOrganisationDialog = this._modalService.show(
        CreateOrganisationComponent,
        {
          class: 'modal-lg',
        }
      );
    } else {
      createOrEditOrganisationDialog = this._modalService.show(
        EditOrganisationComponent,
        {
          class: 'modal-lg',
          initialState: {
            id: id,
          },
        }
      );
    }

    createOrEditOrganisationDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
}

import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CropApiService, FieldDto } from '../services/crop-api.service';
import { CreateEditFieldComponent } from './create-edit-field/create-edit-field.component';

@Component({
  templateUrl: './field-list.component.html',
  animations: [appModuleAnimation()],
})
export class FieldListComponent extends AppComponentBase implements OnInit {
  fields: FieldDto[] = [];
  loading = true;
  canManage = false;

  constructor(
    injector: Injector,
    private cropApi: CropApiService,
    private modalService: BsModalService,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.canManage = this.permission.isGranted('Pages.Fields.Manage');
    this.load();
  }

  load(): void {
    this.loading = true;
    this.cropApi.getFields().subscribe({
      next: (r) => {
        this.fields = r.items;
        this.loading = false;
        this.cd.detectChanges();
      },
      error: () => {
        this.fields = [];
        this.loading = false;
        this.cd.detectChanges();
      },
    });
  }

  create(): void {
    const ref: BsModalRef = this.modalService.show(CreateEditFieldComponent, { class: 'modal-lg' });
    ref.content.onSave.subscribe(() => this.load());
  }

  edit(field: FieldDto): void {
    const ref: BsModalRef = this.modalService.show(CreateEditFieldComponent, { class: 'modal-lg', initialState: { id: field.id } });
    ref.content.onSave.subscribe(() => this.load());
  }

  delete(field: FieldDto): void {
    abp.message.confirm(this.l('FieldDeleteWarningMessage', field.name), undefined, (result: boolean) => {
      if (result) {
        this.cropApi.deleteField(field.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.load();
        });
      }
    });
  }
}

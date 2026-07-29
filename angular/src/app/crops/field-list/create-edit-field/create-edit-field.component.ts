import { ChangeDetectorRef, Component, Injector, OnInit, output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { FacilityDto, FacilityServiceProxy } from '@shared/service-proxies/service-proxies';
import { CropApiService, FieldDto } from '../../services/crop-api.service';

@Component({
  templateUrl: './create-edit-field.component.html',
})
export class CreateEditFieldComponent extends AppComponentBase implements OnInit {
  id: string | undefined;
  saving = false;
  facilities: FacilityDto[] = [];

  field: { facilityId?: string; name: string; areaHectares?: number; soilType?: string } = { name: '' };

  onSave = output<void>();

  constructor(
    injector: Injector,
    private facilityService: FacilityServiceProxy,
    private cropApi: CropApiService,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  get isEdit(): boolean {
    return !!this.id;
  }

  ngOnInit(): void {
    this.facilityService.getAll(undefined, true, 0, 200).subscribe((r) => {
      this.facilities = r.items;
      this.cd.detectChanges();
    });

    if (this.isEdit) {
      this.cropApi.getField(this.id!).subscribe((field) => {
        this.field = {
          facilityId: field.facility?.id,
          name: field.name,
          areaHectares: field.areaHectares,
          soilType: field.soilType,
        };
        this.cd.detectChanges();
      });
    }
  }

  save(): void {
    this.saving = true;

    if (this.isEdit) {
      this.cropApi
        .updateField({ id: this.id!, name: this.field.name, areaHectares: this.field.areaHectares, soilType: this.field.soilType })
        .subscribe({
          next: () => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.bsModalRef.hide();
            this.onSave.emit();
          },
          error: () => {
            this.saving = false;
            this.cd.detectChanges();
          },
        });
      return;
    }

    if (!this.field.facilityId) {
      this.saving = false;
      return;
    }

    this.cropApi
      .createField({
        facilityId: this.field.facilityId,
        name: this.field.name,
        areaHectares: this.field.areaHectares,
        soilType: this.field.soilType,
      })
      .subscribe({
        next: () => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => {
          this.saving = false;
          this.cd.detectChanges();
        },
      });
  }
}

import { ChangeDetectorRef, Component, Injector, OnInit, output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CropApiService, CropTypeDto, FieldDto, SeedVarietyDto } from '../../services/crop-api.service';

@Component({
  templateUrl: './plant-crop-season.component.html',
})
export class PlantCropSeasonComponent extends AppComponentBase implements OnInit {
  fieldId: string | undefined;
  saving = false;
  fields: FieldDto[] = [];
  cropTypes: CropTypeDto[] = [];
  seedVarieties: SeedVarietyDto[] = [];
  filteredVarieties: SeedVarietyDto[] = [];

  input: {
    fieldId?: string;
    cropTypeId?: string;
    seedVarietyId?: string;
    plantingDate?: string;
    expectedHarvestDate?: string;
    expectedYieldKg?: number;
    plantPopulationPerHectare?: number;
  } = {};

  onSave = output<void>();

  constructor(injector: Injector, private cropApi: CropApiService, public bsModalRef: BsModalRef, private cd: ChangeDetectorRef) {
    super(injector);
  }

  ngOnInit(): void {
    this.input.fieldId = this.fieldId;

    this.cropApi.getFields().subscribe((r) => {
      this.fields = r.items;
      this.cd.detectChanges();
    });

    this.cropApi.getCropTypes().subscribe((r) => {
      this.cropTypes = r.items.filter((c) => c.isActive);
      this.cd.detectChanges();
    });

    this.cropApi.getSeedVarieties().subscribe((r) => {
      this.seedVarieties = r.items;
      this.onCropTypeChange();
      this.cd.detectChanges();
    });
  }

  onCropTypeChange(): void {
    this.filteredVarieties = this.seedVarieties.filter((v) => v.cropType?.id === this.input.cropTypeId);
    if (!this.filteredVarieties.some((v) => v.id === this.input.seedVarietyId)) {
      this.input.seedVarietyId = undefined;
    }
  }

  save(): void {
    if (!this.input.fieldId || !this.input.cropTypeId || !this.input.plantingDate || !this.input.expectedHarvestDate) {
      return;
    }

    this.saving = true;
    this.cropApi
      .plantSeason({
        fieldId: this.input.fieldId,
        cropTypeId: this.input.cropTypeId,
        seedVarietyId: this.input.seedVarietyId,
        plantingDate: this.input.plantingDate,
        expectedHarvestDate: this.input.expectedHarvestDate,
        expectedYieldKg: this.input.expectedYieldKg,
        plantPopulationPerHectare: this.input.plantPopulationPerHectare,
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

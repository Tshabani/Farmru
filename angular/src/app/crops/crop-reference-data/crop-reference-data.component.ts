import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { CropApiService, CropTypeDto, SeedSupplierDto, SeedVarietyDto } from '../services/crop-api.service';

@Component({
  templateUrl: './crop-reference-data.component.html',
  animations: [appModuleAnimation()],
})
export class CropReferenceDataComponent extends AppComponentBase implements OnInit {
  loading = true;
  canManage = false;
  activeTab: 'cropTypes' | 'seedVarieties' | 'seedSuppliers' = 'cropTypes';

  cropTypes: CropTypeDto[] = [];
  seedSuppliers: SeedSupplierDto[] = [];
  seedVarieties: SeedVarietyDto[] = [];

  newCropType: { name: string; scientificName?: string; typicalGrowthDurationDays?: number } = { name: '' };
  editingCropType: CropTypeDto | undefined;

  newSeedSupplier: { name: string; contactInfo?: string } = { name: '' };
  editingSeedSupplier: SeedSupplierDto | undefined;

  newSeedVariety: { cropTypeId?: string; supplierId?: string; name: string; daysToMaturity?: number } = { name: '' };

  constructor(injector: Injector, private cropApi: CropApiService, private cd: ChangeDetectorRef) {
    super(injector);
  }

  ngOnInit(): void {
    this.canManage = this.permission.isGranted('Pages.Crops.Manage');
    this.load();
  }

  load(): void {
    this.loading = true;
    this.cropApi.getCropTypes().subscribe((r) => {
      this.cropTypes = r.items;
      this.finishLoadStep();
    });
    this.cropApi.getSeedSuppliers().subscribe((r) => {
      this.seedSuppliers = r.items;
      this.finishLoadStep();
    });
    this.cropApi.getSeedVarieties().subscribe((r) => {
      this.seedVarieties = r.items;
      this.finishLoadStep();
    });
  }

  private loadedCount = 0;
  private finishLoadStep(): void {
    this.loadedCount++;
    if (this.loadedCount >= 3) {
      this.loading = false;
      this.loadedCount = 0;
    }
    this.cd.detectChanges();
  }

  setTab(tab: 'cropTypes' | 'seedVarieties' | 'seedSuppliers'): void {
    this.activeTab = tab;
  }

  // Crop Types
  createCropType(): void {
    if (!this.newCropType.name || !this.newCropType.typicalGrowthDurationDays) {
      return;
    }
    this.cropApi
      .createCropType({
        name: this.newCropType.name,
        scientificName: this.newCropType.scientificName,
        typicalGrowthDurationDays: this.newCropType.typicalGrowthDurationDays,
      })
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.newCropType = { name: '' };
        this.load();
      });
  }

  editCropType(cropType: CropTypeDto): void {
    this.editingCropType = { ...cropType };
  }

  saveCropType(): void {
    if (!this.editingCropType) {
      return;
    }
    this.cropApi.updateCropType(this.editingCropType).subscribe(() => {
      this.notify.info(this.l('SavedSuccessfully'));
      this.editingCropType = undefined;
      this.load();
    });
  }

  deleteCropType(cropType: CropTypeDto): void {
    abp.message.confirm(this.l('CropTypeDeleteWarningMessage', cropType.name), undefined, (result: boolean) => {
      if (result) {
        this.cropApi.deleteCropType(cropType.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.load();
        });
      }
    });
  }

  // Seed Suppliers
  createSeedSupplier(): void {
    if (!this.newSeedSupplier.name) {
      return;
    }
    this.cropApi.createSeedSupplier(this.newSeedSupplier).subscribe(() => {
      this.notify.info(this.l('SavedSuccessfully'));
      this.newSeedSupplier = { name: '' };
      this.load();
    });
  }

  editSeedSupplier(supplier: SeedSupplierDto): void {
    this.editingSeedSupplier = { ...supplier };
  }

  saveSeedSupplier(): void {
    if (!this.editingSeedSupplier) {
      return;
    }
    this.cropApi.updateSeedSupplier(this.editingSeedSupplier).subscribe(() => {
      this.notify.info(this.l('SavedSuccessfully'));
      this.editingSeedSupplier = undefined;
      this.load();
    });
  }

  deleteSeedSupplier(supplier: SeedSupplierDto): void {
    abp.message.confirm(this.l('SeedSupplierDeleteWarningMessage', supplier.name), undefined, (result: boolean) => {
      if (result) {
        this.cropApi.deleteSeedSupplier(supplier.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.load();
        });
      }
    });
  }

  // Seed Varieties
  createSeedVariety(): void {
    if (!this.newSeedVariety.name || !this.newSeedVariety.cropTypeId) {
      return;
    }
    this.cropApi
      .createSeedVariety({
        cropTypeId: this.newSeedVariety.cropTypeId,
        supplierId: this.newSeedVariety.supplierId,
        name: this.newSeedVariety.name,
        daysToMaturity: this.newSeedVariety.daysToMaturity,
      })
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.newSeedVariety = { name: '' };
        this.load();
      });
  }

  deleteSeedVariety(variety: SeedVarietyDto): void {
    abp.message.confirm(this.l('SeedVarietyDeleteWarningMessage', variety.name), undefined, (result: boolean) => {
      if (result) {
        this.cropApi.deleteSeedVariety(variety.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.load();
        });
      }
    });
  }
}

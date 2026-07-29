import { ChangeDetectorRef, Component, Injector, output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CropApiService } from '../../services/crop-api.service';

@Component({
  templateUrl: './harvest-season.component.html',
})
export class HarvestSeasonComponent extends AppComponentBase {
  cropSeasonId: string;
  saving = false;
  input: { harvestDate?: string; actualYieldKg?: number; qualityGrade?: string } = {};

  onSave = output<void>();

  constructor(injector: Injector, private cropApi: CropApiService, public bsModalRef: BsModalRef, private cd: ChangeDetectorRef) {
    super(injector);
  }

  save(): void {
    if (!this.input.harvestDate || this.input.actualYieldKg == null) {
      return;
    }
    this.saving = true;
    this.cropApi
      .harvestSeason({
        cropSeasonId: this.cropSeasonId,
        harvestDate: this.input.harvestDate,
        actualYieldKg: this.input.actualYieldKg,
        qualityGrade: this.input.qualityGrade,
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

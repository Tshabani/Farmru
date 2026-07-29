import { ChangeDetectorRef, Component, Injector, output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CropApiService } from '../../services/crop-api.service';

@Component({
  templateUrl: './log-growth-stage.component.html',
})
export class LogGrowthStageComponent extends AppComponentBase {
  cropSeasonId: string;
  saving = false;
  input: { stage?: number; observedDate?: string } = {};

  stageOptions = [
    { value: 0, label: 'Planted' },
    { value: 1, label: 'Germination' },
    { value: 2, label: 'Vegetative' },
    { value: 3, label: 'Flowering' },
    { value: 4, label: 'Fruiting' },
    { value: 5, label: 'Maturity' },
  ];

  onSave = output<void>();

  constructor(injector: Injector, private cropApi: CropApiService, public bsModalRef: BsModalRef, private cd: ChangeDetectorRef) {
    super(injector);
  }

  save(): void {
    if (this.input.stage == null || !this.input.observedDate) {
      return;
    }
    this.saving = true;
    this.cropApi.logGrowthStage({ cropSeasonId: this.cropSeasonId, stage: this.input.stage, observedDate: this.input.observedDate }).subscribe({
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

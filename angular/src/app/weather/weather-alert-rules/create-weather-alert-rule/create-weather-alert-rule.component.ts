import { ChangeDetectorRef, Component, Injector, output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { WeatherApiService } from '../../services/weather-api.service';

@Component({
  templateUrl: './create-weather-alert-rule.component.html',
})
export class CreateWeatherAlertRuleComponent extends AppComponentBase {
  facilityId: string;
  saving = false;

  alertTypeOptions = [
    { value: 0, label: 'Frost' },
    { value: 1, label: 'Wind' },
    { value: 2, label: 'Heat' },
    { value: 3, label: 'Lightning' },
    { value: 4, label: 'RainSevere' },
  ];

  severityOptions = [
    { value: 0, label: 'Info' },
    { value: 1, label: 'Warning' },
    { value: 2, label: 'Critical' },
  ];

  input: { alertType?: number; thresholdValue?: number; severity?: number } = { severity: 1 };

  onSave = output<void>();

  constructor(injector: Injector, private weatherApi: WeatherApiService, public bsModalRef: BsModalRef, private cd: ChangeDetectorRef) {
    super(injector);
  }

  save(): void {
    if (this.input.alertType == null || this.input.thresholdValue == null || this.input.severity == null) {
      return;
    }
    this.saving = true;
    this.weatherApi
      .createAlertRule({
        facilityId: this.facilityId,
        alertType: this.input.alertType,
        thresholdValue: this.input.thresholdValue,
        severity: this.input.severity,
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

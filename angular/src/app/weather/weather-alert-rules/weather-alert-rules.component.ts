import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { FacilityDto, FacilityServiceProxy } from '@shared/service-proxies/service-proxies';
import { WeatherApiService, WeatherAlertRuleDto } from '../services/weather-api.service';
import { CreateWeatherAlertRuleComponent } from './create-weather-alert-rule/create-weather-alert-rule.component';

@Component({
  templateUrl: './weather-alert-rules.component.html',
  animations: [appModuleAnimation()],
})
export class WeatherAlertRulesComponent extends AppComponentBase implements OnInit {
  facilities: FacilityDto[] = [];
  selectedFacilityId: string | undefined;
  rules: WeatherAlertRuleDto[] = [];
  loading = true;
  canConfigure = false;

  alertTypeLabels = ['Frost', 'Wind', 'Heat', 'Lightning', 'RainSevere'];
  severityLabels = ['Info', 'Warning', 'Critical'];

  constructor(
    injector: Injector,
    private facilityService: FacilityServiceProxy,
    private weatherApi: WeatherApiService,
    private modalService: BsModalService,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.canConfigure = this.permission.isGranted('Pages.Weather.Configure');
    this.facilityService.getAll(undefined, true, 0, 200).subscribe((r) => {
      this.facilities = r.items;
      this.selectedFacilityId = this.facilities[0]?.id;
      if (this.selectedFacilityId) {
        this.load();
      } else {
        this.loading = false;
        this.cd.detectChanges();
      }
    });
  }

  onFacilityChange(): void {
    this.load();
  }

  load(): void {
    if (!this.selectedFacilityId) {
      return;
    }
    this.loading = true;
    this.weatherApi.getAlertRulesForFacility(this.selectedFacilityId).subscribe({
      next: (rules) => {
        this.rules = rules;
        this.loading = false;
        this.cd.detectChanges();
      },
      error: () => {
        this.rules = [];
        this.loading = false;
        this.cd.detectChanges();
      },
    });
  }

  create(): void {
    if (!this.selectedFacilityId) {
      return;
    }
    const ref: BsModalRef = this.modalService.show(CreateWeatherAlertRuleComponent, {
      initialState: { facilityId: this.selectedFacilityId },
    });
    ref.content.onSave.subscribe(() => this.load());
  }

  deactivate(rule: WeatherAlertRuleDto): void {
    abp.message.confirm(this.l('DeactivateWeatherAlertRuleConfirmMessage'), undefined, (result: boolean) => {
      if (result) {
        this.weatherApi.deactivateAlertRule(rule.id).subscribe(() => {
          abp.notify.success(this.l('SavedSuccessfully'));
          this.load();
        });
      }
    });
  }
}

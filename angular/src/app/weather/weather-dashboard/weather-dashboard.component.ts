import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FacilityDto, FacilityServiceProxy } from '@shared/service-proxies/service-proxies';
import { WeatherApiService, WeatherObservationDto, WeatherForecastDto } from '../services/weather-api.service';

@Component({
  templateUrl: './weather-dashboard.component.html',
  animations: [appModuleAnimation()],
})
export class WeatherDashboardComponent extends AppComponentBase implements OnInit {
  facilities: FacilityDto[] = [];
  selectedFacilityId: string | undefined;
  current: WeatherObservationDto | undefined;
  forecast: WeatherForecastDto[] = [];
  loading = true;
  canConfigure = false;

  frostRiskLabels = ['None', 'Watch', 'Warning'];
  heatStressLabels = ['None', 'Elevated', 'Severe'];

  constructor(
    injector: Injector,
    private facilityService: FacilityServiceProxy,
    private weatherApi: WeatherApiService,
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
    this.weatherApi.getCurrent(this.selectedFacilityId).subscribe({
      next: (current) => {
        this.current = current;
        this.cd.detectChanges();
      },
      error: () => {
        this.current = undefined;
        this.cd.detectChanges();
      },
    });

    this.weatherApi.getForecast(this.selectedFacilityId).subscribe({
      next: (forecast) => {
        this.forecast = forecast;
        this.loading = false;
        this.cd.detectChanges();
      },
      error: () => {
        this.forecast = [];
        this.loading = false;
        this.cd.detectChanges();
      },
    });
  }
}

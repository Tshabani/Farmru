import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { MonitoringApiService, MonitoringConfigurationDto } from '../services/monitoring-api.service';

@Component({ templateUrl: './monitoring-config.component.html' })
export class MonitoringConfigComponent extends AppComponentBase implements OnInit {
  config: MonitoringConfigurationDto;
  loading = true;
  saving = false;

  constructor(
    injector: Injector,
    private monitoringApi: MonitoringApiService,
    private router: Router,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.monitoringApi.getConfiguration().subscribe((c) => {
      this.config = c;
      this.loading = false;
      this.cd.detectChanges();
    });
  }

  save(): void {
    this.saving = true;
    this.monitoringApi.updateConfiguration(this.config).subscribe(() => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.saving = false;
      this.cd.detectChanges();
    });
  }

  back(): void {
    this.router.navigate(['/app/monitoring']);
  }
}

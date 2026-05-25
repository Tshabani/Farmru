import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { MonitoringApiService, MonitoringDashboardDto } from '../services/monitoring-api.service';

@Component({
  selector: 'app-monitoring-status-widget',
  template: `
    <div class="card card-outline card-secondary" *ngIf="dashboard">
      <div class="card-header py-2">
        <h3 class="card-title mb-0">{{ 'MonitoringEngine' | localize }}</h3>
        <div class="card-tools">
          <a routerLink="/app/monitoring" class="btn btn-tool btn-sm">Open</a>
        </div>
      </div>
      <div class="card-body py-2 small">
        <span class="badge badge-success mr-1">{{ dashboard.onlineDevices }} online</span>
        <span class="badge badge-danger mr-1">{{ dashboard.offlineDevices }} offline</span>
        <span class="badge badge-warning">{{ dashboard.staleTelemetryDevices }} stale</span>
      </div>
    </div>
  `,
})
export class MonitoringStatusWidgetComponent extends AppComponentBase implements OnInit {
  dashboard: MonitoringDashboardDto;

  constructor(
    injector: Injector,
    private monitoringApi: MonitoringApiService,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.monitoringApi.getDashboard().subscribe((d) => {
      this.dashboard = d;
      this.cd.detectChanges();
    });
  }
}

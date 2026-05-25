import { ChangeDetectorRef, Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { MonitoringApiService, MonitoringDashboardDto, MonitoringExecutionHistoryDto } from '../services/monitoring-api.service';
import { AlertSignalRService } from '../../alerts/services/alert-signalr.service';
import { Subscription } from 'rxjs';

@Component({
  templateUrl: './monitoring-dashboard.component.html',
  animations: [appModuleAnimation()],
})
export class MonitoringDashboardComponent extends AppComponentBase implements OnInit, OnDestroy {
  dashboard: MonitoringDashboardDto;
  history: MonitoringExecutionHistoryDto[] = [];
  feed: { eventType: string; message: string; occurredAt: string }[] = [];
  loading = true;
  private sub: Subscription;

  jobLabels = ['Device offline', 'Telemetry health', 'Alert escalation', 'Full cycle'];

  constructor(
    injector: Injector,
    private monitoringApi: MonitoringApiService,
    private alertSignalR: AlertSignalRService,
    private router: Router,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.alertSignalR.start();
    this.alertSignalR.alertChanged$.subscribe((e) => {
      this.feed.unshift({
        eventType: e.action,
        message: e.alert?.title ?? '',
        occurredAt: new Date().toISOString(),
      });
      if (this.feed.length > 50) {
        this.feed.pop();
      }
      this.cd.detectChanges();
    });
    this.sub = this.alertSignalR.monitoringEvent$.subscribe((e) => {
      this.feed.unshift({
        eventType: e.eventType,
        message: e.message,
        occurredAt: e.occurredAt ?? new Date().toISOString(),
      });
      if (this.feed.length > 50) {
        this.feed.pop();
      }
      this.cd.detectChanges();
    });
    this.alertSignalR.monitoringSummary$.subscribe(() => this.load());
    this.load();
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  load(): void {
    this.loading = true;
    this.monitoringApi.getDashboard().subscribe((d) => {
      this.dashboard = d;
      this.monitoringApi.getExecutionHistory(0, 10).subscribe((h) => {
        this.history = h.items;
        this.loading = false;
        this.cd.detectChanges();
      });
    });
  }

  openConfig(): void {
    this.router.navigate(['/app/monitoring/config']);
  }

  openAlerts(): void {
    this.router.navigate(['/app/alerts']);
  }
}

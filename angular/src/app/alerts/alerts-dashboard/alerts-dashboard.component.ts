import { ChangeDetectorRef, Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AlertApiService, AlertDto, AlertStatisticsDto } from '../services/alert-api.service';
import { AlertSignalRService } from '../services/alert-signalr.service';
import { Subscription } from 'rxjs';
@Component({
  templateUrl: './alerts-dashboard.component.html',
  animations: [appModuleAnimation()],
})
export class AlertsDashboardComponent extends AppComponentBase implements OnInit, OnDestroy {
  stats: AlertStatisticsDto;
  activeAlerts: AlertDto[] = [];
  criticalAlerts: AlertDto[] = [];
  loading = true;
  canManage = false;
  private sub: Subscription;

  severityLabels = ['Info', 'Warning', 'Critical'];
  typeLabels = [
    'Device offline', 'Low battery', 'Low moisture', 'High temperature',
    'Low temperature', 'Telemetry anomaly', 'Geo-fence', 'Sensor failure',
  ];

  constructor(
    injector: Injector,
    private alertApi: AlertApiService,
    private alertSignalR: AlertSignalRService,
    private router: Router,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.canManage = this.permission.isGranted('Pages.Alerts.Manage');
    this.alertSignalR.start();
    this.sub = this.alertSignalR.alertChanged$.subscribe((e) => {
      if (e.action === 'created' && e.alert.severity === 2) {
        abp.notify.warn(e.alert.title, 'Critical alert');
      }
      this.load();
    });
    this.load();
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  load(): void {
    this.loading = true;
    this.alertApi.getStatistics().subscribe((stats) => {
      this.stats = stats;
      this.alertApi.getActiveAlerts().subscribe((active) => {
        this.activeAlerts = active;
        this.alertApi.getCriticalAlerts().subscribe((critical) => {
          this.criticalAlerts = critical;
          this.loading = false;
          this.cd.detectChanges();
        });
      });
    });
  }

  severityClass(severity: number): string {
    return ['badge-info', 'badge-warning', 'badge-danger'][severity] ?? 'badge-secondary';
  }

  openDetail(alert: AlertDto): void {
    this.router.navigate(['/app/alerts', alert.id]);
  }

  acknowledge(alert: AlertDto, event: Event): void {
    event.stopPropagation();
    this.alertApi.acknowledge(alert.id).subscribe(() => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.load();
    });
  }

  resolve(alert: AlertDto, event: Event): void {
    event.stopPropagation();
    this.alertApi.resolve(alert.id).subscribe(() => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.load();
    });
  }

  viewAll(): void {
    this.router.navigate(['/app/alerts/list']);
  }
}

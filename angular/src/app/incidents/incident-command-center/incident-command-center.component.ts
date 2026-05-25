import { ChangeDetectorRef, Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { IncidentApiService, IncidentDashboardDto, IncidentDto } from '../services/incident-api.service';
import { AlertSignalRService } from '../../alerts/services/alert-signalr.service';
import { Subscription } from 'rxjs';

@Component({
  templateUrl: './incident-command-center.component.html',
  animations: [appModuleAnimation()],
})
export class IncidentCommandCenterComponent extends AppComponentBase implements OnInit, OnDestroy {
  dashboard: IncidentDashboardDto;
  loading = true;
  canManage = false;
  private sub: Subscription;

  statusLabels = ['Open', 'Assigned', 'In progress', 'Waiting on parts', 'Escalated', 'Resolved', 'Closed', 'Cancelled'];
  priorityLabels = ['Low', 'Medium', 'High', 'Critical'];

  constructor(
    injector: Injector,
    private incidentApi: IncidentApiService,
    private signalR: AlertSignalRService,
    private router: Router,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.canManage = this.permission.isGranted('Pages.Incidents.Manage');
    this.signalR.start();
    this.sub = this.signalR.incidentChanged$.subscribe((e) => {
      if (e.action === 'slaBreached') {
        abp.notify.warn(e.incident.title, 'SLA breach');
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
    this.incidentApi.getDashboard().subscribe((d) => {
      this.dashboard = d;
      this.loading = false;
      this.cd.detectChanges();
    });
  }

  openIncident(incident: IncidentDto): void {
    this.router.navigate(['/app/incidents', incident.id]);
  }

  goKanban(): void {
    this.router.navigate(['/app/incidents/kanban']);
  }

  goDispatch(): void {
    this.router.navigate(['/app/incidents/dispatch']);
  }

  priorityClass(priority: number): string {
    return ['badge-secondary', 'badge-info', 'badge-warning', 'badge-danger'][priority] ?? 'badge-secondary';
  }

  slaClass(slaStatus: number): string {
    return ['badge-success', 'badge-warning', 'badge-danger', 'badge-info', 'badge-secondary'][slaStatus] ?? 'badge-secondary';
  }
}

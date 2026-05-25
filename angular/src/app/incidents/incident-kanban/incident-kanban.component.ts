import { ChangeDetectorRef, Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { IncidentApiService, IncidentDashboardDto, IncidentDto, IncidentKanbanColumnDto } from '../services/incident-api.service';
import { AlertSignalRService } from '../../alerts/services/alert-signalr.service';
import { Subscription } from 'rxjs';

@Component({
  templateUrl: './incident-kanban.component.html',
  animations: [appModuleAnimation()],
})
export class IncidentKanbanComponent extends AppComponentBase implements OnInit, OnDestroy {
  columns: IncidentKanbanColumnDto[] = [];
  loading = true;
  canManage = false;
  private sub: Subscription;

  columnTitles = ['Open', 'Assigned', 'In progress', 'Escalated', 'Resolved', 'Closed'];
  columnStatuses = [0, 1, 2, 4, 5, 6];

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
    this.sub = this.signalR.incidentChanged$.subscribe(() => this.load());
    this.load();
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  load(): void {
    this.loading = true;
    this.incidentApi.getDashboard().subscribe((d: IncidentDashboardDto) => {
      this.columns = d.kanban ?? [];
      this.loading = false;
      this.cd.detectChanges();
    });
  }

  columnTitle(status: number): string {
    const idx = this.columnStatuses.indexOf(status);
    return idx >= 0 ? this.columnTitles[idx] : 'Unknown';
  }

  open(incident: IncidentDto): void {
    this.router.navigate(['/app/incidents', incident.id]);
  }

  advance(incident: IncidentDto, targetStatus: number, event: Event): void {
    event.stopPropagation();
    if (!this.canManage) {
      return;
    }
    this.incidentApi.changeStatus({ incidentId: incident.id, status: targetStatus }).subscribe(() => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.load();
    });
  }

  nextStatus(current: number): number | null {
    const flow: Record<number, number> = { 0: 2, 1: 2, 2: 5, 4: 5, 5: 6 };
    return flow[current] ?? null;
  }
}

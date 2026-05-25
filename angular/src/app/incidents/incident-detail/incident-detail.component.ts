import { ChangeDetectorRef, Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { IncidentApiService, IncidentDetailDto } from '../services/incident-api.service';
import { AlertSignalRService } from '../../alerts/services/alert-signalr.service';
import { Subscription } from 'rxjs';

@Component({
  templateUrl: './incident-detail.component.html',
  animations: [appModuleAnimation()],
})
export class IncidentDetailComponent extends AppComponentBase implements OnInit, OnDestroy {
  incident: IncidentDetailDto;
  loading = true;
  comment = '';
  resolveNotes = '';
  canManage = false;
  private sub: Subscription;

  statusLabels = ['Open', 'Assigned', 'In progress', 'Waiting on parts', 'Escalated', 'Resolved', 'Closed', 'Cancelled'];

  constructor(
    injector: Injector,
    private route: ActivatedRoute,
    private incidentApi: IncidentApiService,
    private signalR: AlertSignalRService,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.canManage = this.permission.isGranted('Pages.Incidents.Manage');
    this.signalR.start();
    this.sub = this.signalR.incidentChanged$.subscribe(() => this.load());
    this.route.params.subscribe(() => this.load());
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  load(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      return;
    }
    this.loading = true;
    this.incidentApi.getIncident(id).subscribe((d) => {
      this.incident = d;
      this.loading = false;
      this.cd.detectChanges();
    });
  }

  acknowledge(): void {
    this.incidentApi.acknowledge(this.incident.id).subscribe(() => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.load();
    });
  }

  startWork(): void {
    this.incidentApi.startWork(this.incident.id).subscribe(() => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.load();
    });
  }

  resolve(): void {
    this.incidentApi.resolve(this.incident.id, this.resolveNotes).subscribe(() => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.load();
    });
  }

  escalate(): void {
    this.incidentApi.escalate(this.incident.id, this.comment).subscribe(() => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.load();
    });
  }

  postComment(): void {
    if (!this.comment?.trim()) {
      return;
    }
    this.incidentApi.addComment(this.incident.id, this.comment).subscribe(() => {
      this.comment = '';
      this.notify.success(this.l('SavedSuccessfully'));
      this.load();
    });
  }
}

import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { AlertApiService, AlertDto } from '../services/alert-api.service';
@Component({ templateUrl: './alert-detail.component.html' })
export class AlertDetailComponent extends AppComponentBase implements OnInit {
  alert: AlertDto;
  loading = true;
  canManage = false;
  resolutionNotes = '';
  severityLabels = ['Info', 'Warning', 'Critical'];

  constructor(
    injector: Injector,
    private route: ActivatedRoute,
    private router: Router,
    private alertApi: AlertApiService,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.canManage = this.permission.isGranted('Pages.Alerts.Manage');
    const id = this.route.snapshot.paramMap.get('id');
    this.alertApi.getAlert(id).subscribe((a) => {
      this.alert = a;
      this.loading = false;
      this.cd.detectChanges();
    });
  }

  back(): void {
    this.router.navigate(['/app/alerts']);
  }

  reload(): void {
    this.alertApi.getAlert(this.alert.id).subscribe((a) => {
      this.alert = a;
      this.cd.detectChanges();
    });
  }

  acknowledge(): void {
    this.alertApi.acknowledge(this.alert.id).subscribe(() => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.reload();
    });
  }

  resolve(): void {
    this.alertApi.resolve(this.alert.id, this.resolutionNotes).subscribe(() => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.router.navigate(['/app/alerts']);
    });
  }

  severityClass(severity: number): string {
    return ['badge-info', 'badge-warning', 'badge-danger'][severity] ?? 'badge-secondary';
  }
}

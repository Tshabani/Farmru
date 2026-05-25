import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { AlertApiService, AlertDto } from '../services/alert-api.service';

@Component({ templateUrl: './alerts-list.component.html' })
export class AlertsListComponent extends PagedListingComponentBase<AlertDto> implements OnInit {
  alerts: AlertDto[] = [];
  severityFilter: number | undefined;
  activeOnly = true;

  constructor(
    injector: Injector,
    private alertApi: AlertApiService,
    private router: Router,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.alertApi
      .getAlerts({
        skipCount: request.skipCount,
        maxResultCount: request.maxResultCount,
        severity: this.severityFilter,
        activeOnly: this.activeOnly,
      })
      .subscribe((r) => {
        this.alerts = r.items;
        this.showPaging({ totalCount: r.totalCount, items: r.items } as any, pageNumber);
        finishedCallback();
        this.cd.detectChanges();
      });
  }

  open(alert: AlertDto): void {
    this.router.navigate(['/app/alerts', alert.id]);
  }

  protected delete(_alert: AlertDto): void {
    // Alerts are acknowledged/resolved via detail view, not deleted from the list.
  }
}

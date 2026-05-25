import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { AlertsRoutingModule } from './alerts-routing.module';
import { AlertsDashboardComponent } from './alerts-dashboard/alerts-dashboard.component';
import { AlertsListComponent } from './alerts-list/alerts-list.component';
import { AlertDetailComponent } from './alert-detail/alert-detail.component';
import { FormsModule } from '@angular/forms';
import { NgxPaginationModule } from 'ngx-pagination';
import { MonitoringModule } from '../monitoring/monitoring.module';

@NgModule({
  declarations: [AlertsDashboardComponent, AlertsListComponent, AlertDetailComponent],
  imports: [CommonModule, SharedModule, AlertsRoutingModule, FormsModule, NgxPaginationModule, MonitoringModule],
})
export class AlertsModule {}

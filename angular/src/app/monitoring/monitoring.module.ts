import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { MonitoringRoutingModule } from './monitoring-routing.module';
import { MonitoringDashboardComponent } from './monitoring-dashboard/monitoring-dashboard.component';
import { MonitoringConfigComponent } from './monitoring-config/monitoring-config.component';
import { FormsModule } from '@angular/forms';
import { MonitoringStatusWidgetComponent } from './widgets/monitoring-status-widget.component';

@NgModule({
  declarations: [MonitoringDashboardComponent, MonitoringConfigComponent, MonitoringStatusWidgetComponent],
  imports: [CommonModule, SharedModule, MonitoringRoutingModule, FormsModule],
  exports: [MonitoringStatusWidgetComponent],
})
export class MonitoringModule {}

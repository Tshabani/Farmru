import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MonitoringDashboardComponent } from './monitoring-dashboard/monitoring-dashboard.component';
import { MonitoringConfigComponent } from './monitoring-config/monitoring-config.component';

const routes: Routes = [
  { path: '', component: MonitoringDashboardComponent },
  { path: 'config', component: MonitoringConfigComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class MonitoringRoutingModule {}

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AlertsDashboardComponent } from './alerts-dashboard/alerts-dashboard.component';
import { AlertsListComponent } from './alerts-list/alerts-list.component';
import { AlertDetailComponent } from './alert-detail/alert-detail.component';

const routes: Routes = [
  { path: '', component: AlertsDashboardComponent },
  { path: 'list', component: AlertsListComponent },
  { path: ':id', component: AlertDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AlertsRoutingModule {}

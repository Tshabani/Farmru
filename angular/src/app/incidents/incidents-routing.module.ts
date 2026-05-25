import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IncidentCommandCenterComponent } from './incident-command-center/incident-command-center.component';
import { IncidentKanbanComponent } from './incident-kanban/incident-kanban.component';
import { IncidentDetailComponent } from './incident-detail/incident-detail.component';
import { IncidentDispatchComponent } from './incident-dispatch/incident-dispatch.component';

const routes: Routes = [
  { path: '', component: IncidentCommandCenterComponent },
  { path: 'kanban', component: IncidentKanbanComponent },
  { path: 'dispatch', component: IncidentDispatchComponent },
  { path: ':id', component: IncidentDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class IncidentsRoutingModule {}

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { FormsModule } from '@angular/forms';
import { IncidentsRoutingModule } from './incidents-routing.module';
import { IncidentCommandCenterComponent } from './incident-command-center/incident-command-center.component';
import { IncidentKanbanComponent } from './incident-kanban/incident-kanban.component';
import { IncidentDetailComponent } from './incident-detail/incident-detail.component';
import { IncidentDispatchComponent } from './incident-dispatch/incident-dispatch.component';

@NgModule({
  declarations: [
    IncidentCommandCenterComponent,
    IncidentKanbanComponent,
    IncidentDetailComponent,
    IncidentDispatchComponent,
  ],
  imports: [CommonModule, SharedModule, FormsModule, IncidentsRoutingModule],
})
export class IncidentsModule {}

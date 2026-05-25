import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { GisRoutingModule } from './gis-routing.module';
import { FormsModule } from '@angular/forms';
import { OperationalMapComponent } from './operational-map/operational-map.component';
import { ExecutiveGisComponent } from './executive-gis/executive-gis.component';
import { GeofenceListComponent } from './geofence-list/geofence-list.component';
import { GeofenceEditComponent } from './geofence-edit/geofence-edit.component';

@NgModule({
  declarations: [OperationalMapComponent, ExecutiveGisComponent, GeofenceListComponent, GeofenceEditComponent],
  imports: [CommonModule, SharedModule, GisRoutingModule, FormsModule],
})
export class GisModule {}

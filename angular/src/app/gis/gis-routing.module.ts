import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OperationalMapComponent } from './operational-map/operational-map.component';
import { ExecutiveGisComponent } from './executive-gis/executive-gis.component';
import { GeofenceListComponent } from './geofence-list/geofence-list.component';
import { GeofenceEditComponent } from './geofence-edit/geofence-edit.component';

const routes: Routes = [
  { path: '', component: OperationalMapComponent },
  { path: 'executive', component: ExecutiveGisComponent },
  { path: 'geofences', component: GeofenceListComponent },
  { path: 'geofences/create', component: GeofenceEditComponent },
  { path: 'geofences/:id', component: GeofenceEditComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class GisRoutingModule {}

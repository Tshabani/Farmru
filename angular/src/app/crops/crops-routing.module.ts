import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CropSeasonListComponent } from './crop-season-list/crop-season-list.component';
import { CropSeasonDetailComponent } from './crop-season-detail/crop-season-detail.component';
import { FieldListComponent } from './field-list/field-list.component';
import { CropReferenceDataComponent } from './crop-reference-data/crop-reference-data.component';

const routes: Routes = [
  { path: '', component: CropSeasonListComponent },
  { path: 'fields', component: FieldListComponent },
  { path: 'reference-data', component: CropReferenceDataComponent },
  { path: ':id', component: CropSeasonDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CropsRoutingModule {}

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CropSeasonListComponent } from './crop-season-list/crop-season-list.component';
import { CropSeasonDetailComponent } from './crop-season-detail/crop-season-detail.component';

const routes: Routes = [
  { path: '', component: CropSeasonListComponent },
  { path: ':id', component: CropSeasonDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CropsRoutingModule {}

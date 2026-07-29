import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NutrientDashboardComponent } from './nutrient-dashboard/nutrient-dashboard.component';

const routes: Routes = [{ path: '', component: NutrientDashboardComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class NutrientsRoutingModule {}

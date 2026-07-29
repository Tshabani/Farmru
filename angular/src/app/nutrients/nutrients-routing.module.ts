import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NutrientDashboardComponent } from './nutrient-dashboard/nutrient-dashboard.component';
import { FertilizerProductsComponent } from './fertilizer-products/fertilizer-products.component';

const routes: Routes = [
  { path: '', component: NutrientDashboardComponent },
  { path: 'products', component: FertilizerProductsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class NutrientsRoutingModule {}

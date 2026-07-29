import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { NutrientsRoutingModule } from './nutrients-routing.module';
import { NutrientDashboardComponent } from './nutrient-dashboard/nutrient-dashboard.component';

@NgModule({
  declarations: [NutrientDashboardComponent],
  imports: [CommonModule, SharedModule, ServiceProxyModule, NutrientsRoutingModule, FormsModule],
})
export class NutrientsModule {}

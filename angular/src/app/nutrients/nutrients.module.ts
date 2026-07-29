import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { ModalModule } from 'ngx-bootstrap/modal';
import { NutrientsRoutingModule } from './nutrients-routing.module';
import { NutrientDashboardComponent } from './nutrient-dashboard/nutrient-dashboard.component';
import { RecordApplicationComponent } from './nutrient-dashboard/record-application/record-application.component';
import { FertilizerProductsComponent } from './fertilizer-products/fertilizer-products.component';

@NgModule({
  declarations: [NutrientDashboardComponent, RecordApplicationComponent, FertilizerProductsComponent],
  imports: [CommonModule, SharedModule, ServiceProxyModule, NutrientsRoutingModule, FormsModule, ModalModule.forChild()],
})
export class NutrientsModule {}

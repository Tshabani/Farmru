import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { WeatherRoutingModule } from './weather-routing.module';
import { WeatherDashboardComponent } from './weather-dashboard/weather-dashboard.component';

@NgModule({
  declarations: [WeatherDashboardComponent],
  imports: [CommonModule, SharedModule, ServiceProxyModule, WeatherRoutingModule, FormsModule],
})
export class WeatherModule {}

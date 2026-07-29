import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { ModalModule } from 'ngx-bootstrap/modal';
import { WeatherRoutingModule } from './weather-routing.module';
import { WeatherDashboardComponent } from './weather-dashboard/weather-dashboard.component';
import { WeatherAlertRulesComponent } from './weather-alert-rules/weather-alert-rules.component';
import { CreateWeatherAlertRuleComponent } from './weather-alert-rules/create-weather-alert-rule/create-weather-alert-rule.component';

@NgModule({
  declarations: [WeatherDashboardComponent, WeatherAlertRulesComponent, CreateWeatherAlertRuleComponent],
  imports: [CommonModule, SharedModule, ServiceProxyModule, WeatherRoutingModule, FormsModule, ModalModule.forChild()],
})
export class WeatherModule {}

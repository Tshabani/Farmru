import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WeatherDashboardComponent } from './weather-dashboard/weather-dashboard.component';
import { WeatherAlertRulesComponent } from './weather-alert-rules/weather-alert-rules.component';

const routes: Routes = [
  { path: '', component: WeatherDashboardComponent },
  { path: 'alert-rules', component: WeatherAlertRulesComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class WeatherRoutingModule {}

import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { HomeRoutingModule } from './home-routing.module';
import { HomeComponent } from './home.component';
import { Ng2GoogleChartsModule } from 'ng2-google-charts';
import { AgriculturalDashboardComponent } from '../agricultural-dashboard/agricultural-dashboard.component';
import { CommonModule } from '@node_modules/@angular/common';

@NgModule({
    declarations: [HomeComponent, AgriculturalDashboardComponent],
    imports: [SharedModule, HomeRoutingModule, Ng2GoogleChartsModule, CommonModule ],
})
export class HomeModule {}


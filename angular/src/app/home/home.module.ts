import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { HomeRoutingModule } from './home-routing.module';
import { HomeComponent } from './home.component';
import { Ng2GoogleChartsModule } from 'ng2-google-charts';

@NgModule({
    declarations: [HomeComponent],
    imports: [SharedModule, HomeRoutingModule, Ng2GoogleChartsModule ],
})
export class HomeModule {}


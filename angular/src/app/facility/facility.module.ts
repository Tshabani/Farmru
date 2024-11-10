import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { FacilityRoutingModule } from './facility-routing.module';
import { FacilityComponent } from './facility.component';
import { EditFacilityComponent } from './edit-facility/edit-facility.component';
import { CreateFacilityComponent } from './create-facility/create-facility.component';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '@app/app-routing.module';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { Ng2GoogleChartsModule } from 'ng2-google-charts';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxPaginationModule } from 'ngx-pagination';

@NgModule({
    declarations: [FacilityComponent, EditFacilityComponent, CreateFacilityComponent],
    imports: [
        SharedModule,
        FacilityRoutingModule,
        Ng2GoogleChartsModule,
        AppRoutingModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        ModalModule.forChild(),
        BsDropdownModule,
        CollapseModule,
        TabsModule,
        ServiceProxyModule,
        NgxPaginationModule,
    ],
})
export class FacilityModule { }

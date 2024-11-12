import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { FacilityAppointmentRoutingModule } from './facility-appointment-routing.module';
import { FacilityAppointmentComponent } from './facility-appointment.component';
import { EditFacilityAppointmentComponent } from './edit-facility-appointment/edit-facility-appointment.component';
import { CreateFacilityAppointmentComponent } from './create-facility-appointment/create-facility-appointment.component';
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
    declarations: [FacilityAppointmentComponent, EditFacilityAppointmentComponent, CreateFacilityAppointmentComponent],
    imports: [
        SharedModule,
        FacilityAppointmentRoutingModule,
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
export class FacilityAppointmentModule { }

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { FacilityAppointmentComponent } from './facility-appointment.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: FacilityAppointmentComponent,
            }
        ])
    ],
    exports: [RouterModule]
})
export class FacilityAppointmentRoutingModule { }

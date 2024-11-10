import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FacilityComponent } from './facility.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: FacilityComponent,
            }
        ])
    ],
    exports: [RouterModule]
})
export class FacilityRoutingModule { }

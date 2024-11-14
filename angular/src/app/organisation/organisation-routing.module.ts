import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrganisationComponent } from './organisation.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: OrganisationComponent,

            }
        ])
    ],
    exports: [RouterModule]
})
export class OrganisationRoutingModule { }

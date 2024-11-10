import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PersonComponent } from './person.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: PersonComponent,
            }
        ])
    ],
    exports: [RouterModule]
})
export class PersonRoutingModule { }

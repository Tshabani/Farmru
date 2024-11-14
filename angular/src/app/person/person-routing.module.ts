import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PersonComponent } from './person.component';

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

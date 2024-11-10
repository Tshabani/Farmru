import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { PersonRoutingModule } from './person-routing.module';
import { PersonComponent } from './person.component';
import { EditPersonComponent } from './edit-person/edit-person.component';
import { CreatePersonComponent } from './create-person/create-person.component';
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
    declarations: [PersonComponent, EditPersonComponent, CreatePersonComponent],
    imports: [
        SharedModule,
        PersonRoutingModule,
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
export class PersonModule { }

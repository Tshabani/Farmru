import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { OrganisationRoutingModule } from './organisation-routing.module';
import { OrganisationComponent } from './organisation.component';
import { EditOrganisationComponent } from './edit-organisation/edit-organisation.component';
import { CreateOrganisationComponent } from './create-organisation/create-organisation.component';
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
    declarations: [OrganisationComponent, EditOrganisationComponent, CreateOrganisationComponent],
    imports: [
        SharedModule,
        OrganisationRoutingModule,
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
export class OrganisationModule { }

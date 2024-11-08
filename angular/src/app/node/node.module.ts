import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { NodeRoutingModule } from './node-routing.module';
import { NodeComponent } from './node.component';
import { EditNodeComponent } from './edit-node/edit-node.component';
import { CreateNodeComponent } from './create-node/create-node.component';
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
import { ViewNodeDataComponent } from './view-node-data/view-node-data.component';

@NgModule({
    declarations: [NodeComponent, EditNodeComponent, CreateNodeComponent, ViewNodeDataComponent],
    imports: [
        SharedModule,
        NodeRoutingModule,
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
export class NodeModule { }

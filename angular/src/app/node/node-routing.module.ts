import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NodeComponent } from './node.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { ViewNodeDataComponent } from './view-node-data/view-node-data.component'

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: NodeComponent,

            },
            {
                path: 'nodeData/:id',
                component: ViewNodeDataComponent
            }
        ])
    ],
    exports: [RouterModule]
})
export class NodeRoutingModule { }

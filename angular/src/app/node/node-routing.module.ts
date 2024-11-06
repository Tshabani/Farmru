import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NodeComponent } from './node.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: NodeComponent,
                pathMatch: 'full',
                children: [
                    {
                        path: 'nodeData',
                        loadChildren: () => import('./view-node-data/view-node-data.module').then((m) => m.ViewNodeDataModule),
                        data: { permission: 'Pages.Nodes' },
                        canActivate: [AppRouteGuard]
                    }
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class NodeRoutingModule { }

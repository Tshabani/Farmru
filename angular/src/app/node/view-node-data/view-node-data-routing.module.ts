import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ViewNodeDataComponent } from './view-node-data.component';

const routes: Routes = [
    {
        path: ':id',
        component: ViewNodeDataComponent,
        data: { permission: 'Pages.Nodes' },
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ViewNodeDataRoutingModule {}

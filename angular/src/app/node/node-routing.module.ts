import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NodeComponent } from './node.component';

const routes: Routes = [
    {
        path: '',
        component: NodeComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class NodeRoutingModule {}

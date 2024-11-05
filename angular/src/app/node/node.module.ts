import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { NodeRoutingModule } from './node-routing.module';
import { NodeComponent } from './node.component';
import { EditNodeComponent } from './edit-node/edit-node.component';
import { CreateNodeComponent } from './create-node/create-node.component';

@NgModule({
    declarations: [NodeComponent, EditNodeComponent, CreateNodeComponent],
    imports: [SharedModule, NodeRoutingModule],
})
export class NodeModule {}

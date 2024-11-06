import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { ViewNodeDataRoutingModule } from './view-node-data-routing.module';
import { ViewNodeDataComponent } from './view-node-data.component';

@NgModule({
    declarations: [ViewNodeDataComponent],
    imports: [SharedModule, ViewNodeDataRoutingModule],
})
export class ViewNodeDataModule {}

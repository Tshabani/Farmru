import { Component } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
  selector: 'app-view-node-data',
  templateUrl: './view-node-data.component.html',
  styleUrl: './view-node-data.component.css',
  animations: [appModuleAnimation()]
})
export class ViewNodeDataComponent {

}

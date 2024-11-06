import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output, output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { forEach as _forEach, map as _map } from 'lodash-es';
import { NodeDto, NodeServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  templateUrl: './edit-node.component.html',
})
export class EditNodeComponent extends AppComponentBase
  implements OnInit {
    id: string;
    saving = false;
    node = new NodeDto();  

    @Output() onSave = new EventEmitter<any>();
  
    constructor(
      injector: Injector,
      private _nodeService: NodeServiceProxy,
      public bsModalRef: BsModalRef,
      private cd: ChangeDetectorRef
    ) {
      super(injector);
    }
  
    ngOnInit(): void {
      this._nodeService.get(this.id).subscribe((result) => {
        this.node = result; 
        this.cd.detectChanges();  
      });
    }
      
    save(): void {
      this.saving = true;  
      const node = new NodeDto();
      node.init(this.node);
  
      this._nodeService
        .update(node)
        .subscribe(
          () => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.bsModalRef.hide();   
            this.onSave.emit();         
          },
          () => {
            this.saving = false;
            this.cd.detectChanges();
          }
        );
    }
}

import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateNode, NodeDto, NodeServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({  
  templateUrl: './create-node.component.html',
})
export class CreateNodeComponent extends AppComponentBase
implements OnInit {

  saving = false;
  node = new NodeDto();

  onSave = output<EventEmitter<any>>()

  constructor(
    injector: Injector,
    private _nodeService: NodeServiceProxy,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.cd.detectChanges();
  }
  save(): void {
    this.saving = true;

    const node = new CreateNode();
    node.init(this.node);

    this._nodeService
      .create(node)
      .subscribe(
        () => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit(null);
        },
        () => {
          this.saving = false;
          this.cd.detectChanges();
        }
      );
  }
}

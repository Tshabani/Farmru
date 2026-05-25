import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { NodeServiceProxy, ReplaceNodeInput } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './replace-device.component.html',
})
export class ReplaceDeviceComponent extends AppComponentBase implements OnInit {
  nodeId: string;
  saving = false;
  input = new ReplaceNodeInput();

  @Output() onSave = new EventEmitter<void>();

  constructor(
    injector: Injector,
    private _nodeService: NodeServiceProxy,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.input.nodeId = this.nodeId;
  }

  save(): void {
    this.saving = true;
    this._nodeService.replaceDevice(this.input).subscribe(
      () => {
        this.notify.success(this.l('SavedSuccessfully'));
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

import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  NodeDetailDto,
  NodeServiceProxy,
  ReplaceNodeInput,
} from '@shared/service-proxies/service-proxies';
import { AlertApiService, AlertDto } from '../../alerts/services/alert-api.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ReplaceDeviceComponent } from '../replace-device/replace-device.component';
import { finalize } from 'rxjs/operators';

@Component({
  templateUrl: './device-details.component.html',
  animations: [appModuleAnimation()],
})
export class DeviceDetailsComponent extends AppComponentBase implements OnInit {
  device: NodeDetailDto;
  deviceAlerts: AlertDto[] = [];
  severityLabels = ['Info', 'Warning', 'Critical'];
  loading = true;
  deviceId: string;

  constructor(
    injector: Injector,
    private _nodeService: NodeServiceProxy,
    private _route: ActivatedRoute,
    private _router: Router,
    private _modalService: BsModalService,
    private _alertApi: AlertApiService,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.deviceId = this._route.snapshot.paramMap.get('id');
    this.loadDevice();
  }

  loadDevice(): void {
    this.loading = true;
    this.cd.detectChanges();
    this._nodeService
      .getDetail(this.deviceId)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.cd.detectChanges();
        })
      )
      .subscribe({
        next: (result) => {
          this.device = result;
          this._alertApi.getAlertsByNode(this.deviceId).subscribe({
            next: (alerts) => {
              this.deviceAlerts = alerts;
              this.cd.detectChanges();
            },
            error: () => {
              this.deviceAlerts = [];
              this.cd.detectChanges();
            }
          });
        },
        error: (err) => {
          this.notify.error(this.l('ErrorOccurred'));
          this.cd.detectChanges();
        }
      });
  }

  get healthClass(): string {
    switch (this.device?.healthStatus) {
      case 1:
        return 'badge-success';
      case 2:
        return 'badge-warning';
      case 3:
        return 'badge-danger';
      default:
        return 'badge-secondary';
    }
  }

  get statusLabel(): string {
    const labels = ['Active', 'Inactive', 'Maintenance', 'Decommissioned'];
    return labels[this.device?.deviceStatus] ?? 'Unknown';
  }

  get healthLabel(): string {
    const labels = ['Unknown', 'Healthy', 'Warning', 'Critical'];
    return labels[this.device?.healthStatus] ?? 'Unknown';
  }

  activate(): void {
    this._nodeService.activate(this.deviceId).subscribe(() => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.loadDevice();
    });
  }

  deactivate(): void {
    this._nodeService.deactivate(this.deviceId).subscribe(() => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.loadDevice();
    });
  }

  openReplaceDialog(): void {
    const modal: BsModalRef = this._modalService.show(ReplaceDeviceComponent, {
      class: 'modal-lg',
      initialState: { nodeId: this.deviceId },
    });
    modal.content.onSave.subscribe(() => this.loadDevice());
  }

  viewTelemetry(): void {
    this._router.navigate(['/app/node/nodeData', this.deviceId]);
  }

  back(): void {
    this._router.navigate(['/app/node']);
  }

  severityClass(severity: number): string {
    return ['badge-info', 'badge-warning', 'badge-danger'][severity] ?? 'badge-secondary';
  }
}

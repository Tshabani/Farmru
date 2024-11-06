import { ChangeDetectorRef, Component, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { NodeDataDto, NodeDataDtoPagedResultDto, NodeDataServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalService } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs';

@Component({
  templateUrl: './view-node-data.component.html',
  animations: [appModuleAnimation()]
})

class PagedNodeDataRequestDto extends PagedRequestDto {
  keyword: string;
}

export class ViewNodeDataComponent extends PagedListingComponentBase<NodeDataDto> {
  nodeData: NodeDataDto[] = [];
  keyword = '';

  constructor(
    injector: Injector,
    private _nodesService: NodeDataServiceProxy,
    private _modalService: BsModalService,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
    this.nodeData[0].solarPanelVoltage
  }

  list(
    request: PagedNodeDataRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;

    this._nodesService
      .getAll(request.keyword, true,request.skipCount, request.maxResultCount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: NodeDataDtoPagedResultDto) => {
        this.nodeData = result.items;
        this.showPaging(result, pageNumber);
        this.cd.detectChanges();
      });
  }

  delete(node: NodeDataDto): void {
    abp.message.confirm(
      this.l('NodeDeleteWarningMessage', node.id),
      undefined,
      (result: boolean) => {
        if (result) {
          this._nodesService
            .delete(node.id)
            .pipe(
              finalize(() => {
                abp.notify.success(this.l('SuccessfullyDeleted'));
                this.refresh();
              })
            )
            .subscribe(() => {});
        }
      }
    );
  }
}

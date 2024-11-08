import { Component, Injector, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NodeDto, NodeDtoPagedResultDto, NodeServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { EditNodeComponent } from './edit-node/edit-node.component';
import { CreateNodeComponent } from './create-node/create-node.component';
import { ActivatedRoute, Router } from '@angular/router';

class PagedNodesRequestDto extends PagedRequestDto {
  keyword: string;
}

@Component({
  templateUrl: './node.component.html',
  animations: [appModuleAnimation()]
})

export class NodeComponent extends PagedListingComponentBase<NodeDto> {
  nodes: NodeDto[] = [];
  keyword = '';

  constructor(
    injector: Injector,
    private _nodesService: NodeServiceProxy,
    private _modalService: BsModalService,
    private router: Router,
    private route: ActivatedRoute,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }
  onRowClick(rowId: string): void {
    // this.router.navigate(['/nodeData', rowId]);
    this.router.navigate(['nodeData', rowId], { relativeTo: this.route });
  }

  list(
    request: PagedNodesRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;

    this._nodesService
      .getAll(request.keyword, true, request.skipCount, request.maxResultCount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: NodeDtoPagedResultDto) => {
        this.nodes = result.items;
        this.showPaging(result, pageNumber);
        this.cd.detectChanges();
      });
  }

  delete(node: NodeDto): void {
    abp.message.confirm(
      this.l('NodeDeleteWarningMessage', node.serialNumber),
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
            .subscribe(() => { });
        }
      }
    );
  }


  createNode(): void {
    this.showCreateOrEditNodeDialog();
  }

  editNode(node: NodeDto): void {
    this.showCreateOrEditNodeDialog(node.id);
  }

  showCreateOrEditNodeDialog(id?: string): void {
    let createOrEditNodeDialog: BsModalRef;
    if (!id) {
      createOrEditNodeDialog = this._modalService.show(
        CreateNodeComponent,
        {
          class: 'modal-lg',
        }
      );
    } else {
      createOrEditNodeDialog = this._modalService.show(
        EditNodeComponent,
        {
          class: 'modal-lg',
          initialState: {
            id: id,
          },
        }
      );
    }

    createOrEditNodeDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
}


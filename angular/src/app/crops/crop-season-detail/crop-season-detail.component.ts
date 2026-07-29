import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CropApiService, CropSeasonDetailDto } from '../services/crop-api.service';
import { LogGrowthStageComponent } from './log-growth-stage/log-growth-stage.component';
import { HarvestSeasonComponent } from './harvest-season/harvest-season.component';

@Component({
  templateUrl: './crop-season-detail.component.html',
  animations: [appModuleAnimation()],
})
export class CropSeasonDetailComponent extends AppComponentBase implements OnInit {
  season: CropSeasonDetailDto | undefined;
  loading = true;
  canManage = false;
  canHarvest = false;

  statusLabels = ['Planned', 'Growing', 'Harvested', 'Closed'];
  stageLabels = ['Planted', 'Germination', 'Vegetative', 'Flowering', 'Fruiting', 'Maturity', 'Harvested'];

  constructor(
    injector: Injector,
    private route: ActivatedRoute,
    private cropApi: CropApiService,
    private modalService: BsModalService,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.canManage = this.permission.isGranted('Pages.Crops.Manage');
    this.canHarvest = this.permission.isGranted('Pages.Crops.Harvest');
    this.load();
  }

  load(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.loading = false;
      return;
    }
    this.loading = true;
    this.cropApi.getSeasonDetail(id).subscribe({
      next: (season) => {
        this.season = season;
        this.loading = false;
        this.cd.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cd.detectChanges();
      },
    });
  }

  logGrowthStage(): void {
    if (!this.season) {
      return;
    }
    const ref: BsModalRef = this.modalService.show(LogGrowthStageComponent, { initialState: { cropSeasonId: this.season.id } });
    ref.content.onSave.subscribe(() => this.load());
  }

  harvest(): void {
    if (!this.season) {
      return;
    }
    const ref: BsModalRef = this.modalService.show(HarvestSeasonComponent, { initialState: { cropSeasonId: this.season.id } });
    ref.content.onSave.subscribe(() => this.load());
  }

  closeSeason(): void {
    if (!this.season) {
      return;
    }
    abp.message.confirm(this.l('CloseSeasonConfirmMessage'), undefined, (result: boolean) => {
      if (result) {
        this.cropApi.closeSeason(this.season!.id).subscribe(() => {
          abp.notify.success(this.l('SavedSuccessfully'));
          this.load();
        });
      }
    });
  }
}

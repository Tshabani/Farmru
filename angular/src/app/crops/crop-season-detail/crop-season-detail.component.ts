import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { CropApiService, CropSeasonDetailDto } from '../services/crop-api.service';

@Component({
  templateUrl: './crop-season-detail.component.html',
  animations: [appModuleAnimation()],
})
export class CropSeasonDetailComponent extends AppComponentBase implements OnInit {
  season: CropSeasonDetailDto | undefined;
  loading = true;

  statusLabels = ['Planned', 'Growing', 'Harvested', 'Closed'];
  stageLabels = ['Planted', 'Germination', 'Vegetative', 'Flowering', 'Fruiting', 'Maturity', 'Harvested'];

  constructor(injector: Injector, private route: ActivatedRoute, private cropApi: CropApiService, private cd: ChangeDetectorRef) {
    super(injector);
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.loading = false;
      return;
    }
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
}

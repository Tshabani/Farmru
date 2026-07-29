import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CropApiService, CropSeasonDto, FieldDto } from '../services/crop-api.service';
import { PlantCropSeasonComponent } from './plant-crop-season/plant-crop-season.component';

@Component({
  templateUrl: './crop-season-list.component.html',
  animations: [appModuleAnimation()],
})
export class CropSeasonListComponent extends AppComponentBase implements OnInit {
  fields: FieldDto[] = [];
  selectedFieldId: string | undefined;
  seasons: CropSeasonDto[] = [];
  loading = true;
  canManage = false;

  statusLabels = ['Planned', 'Growing', 'Harvested', 'Closed'];

  constructor(
    injector: Injector,
    private cropApi: CropApiService,
    private router: Router,
    private modalService: BsModalService,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.canManage = this.permission.isGranted('Pages.Crops.Manage');
    this.cropApi.getFields().subscribe((r) => {
      this.fields = r.items;
      this.selectedFieldId = this.fields[0]?.id;
      if (this.selectedFieldId) {
        this.load();
      } else {
        this.loading = false;
        this.cd.detectChanges();
      }
    });
  }

  onFieldChange(): void {
    this.load();
  }

  load(): void {
    if (!this.selectedFieldId) {
      return;
    }
    this.loading = true;
    this.cropApi.getSeasonsByField(this.selectedFieldId).subscribe({
      next: (r) => {
        this.seasons = r.items;
        this.loading = false;
        this.cd.detectChanges();
      },
      error: () => {
        this.seasons = [];
        this.loading = false;
        this.cd.detectChanges();
      },
    });
  }

  statusClass(status: number): string {
    return ['fr-chip-yellow', 'fr-chip-green', 'fr-chip-blue', 'fr-chip-grey'][status] ?? 'fr-chip-grey';
  }

  openDetail(season: CropSeasonDto): void {
    this.router.navigate(['/app/crops', season.id]);
  }

  plantSeason(): void {
    const ref: BsModalRef = this.modalService.show(PlantCropSeasonComponent, {
      class: 'modal-lg',
      initialState: { fieldId: this.selectedFieldId },
    });
    ref.content.onSave.subscribe(() => this.load());
  }
}

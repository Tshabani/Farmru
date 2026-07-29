import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { CropApiService, FieldDto } from '../../crops/services/crop-api.service';
import { NutrientApiService, NutrientBalanceSnapshotDto, FertilizerApplicationDto } from '../services/nutrient-api.service';

@Component({
  templateUrl: './nutrient-dashboard.component.html',
  animations: [appModuleAnimation()],
})
export class NutrientDashboardComponent extends AppComponentBase implements OnInit {
  fields: FieldDto[] = [];
  selectedFieldId: string | undefined;
  latest: NutrientBalanceSnapshotDto | undefined;
  applications: FertilizerApplicationDto[] = [];
  loading = true;
  canApply = false;

  statusLabels = ['Deficient', 'Adequate', 'Surplus'];

  constructor(
    injector: Injector,
    private cropApi: CropApiService,
    private nutrientApi: NutrientApiService,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.canApply = this.permission.isGranted('Pages.Nutrients.Apply');
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
    this.nutrientApi.getLatestBalance(this.selectedFieldId).subscribe({
      next: (latest) => {
        this.latest = latest;
        this.cd.detectChanges();
      },
      error: () => {
        this.latest = undefined;
        this.cd.detectChanges();
      },
    });

    this.nutrientApi.getApplicationsByField(this.selectedFieldId).subscribe({
      next: (r) => {
        this.applications = r.items;
        this.loading = false;
        this.cd.detectChanges();
      },
      error: () => {
        this.applications = [];
        this.loading = false;
        this.cd.detectChanges();
      },
    });
  }

  statusClass(status: number): string {
    return ['fr-chip-red', 'fr-chip-green', 'fr-chip-yellow'][status] ?? 'fr-chip-grey';
  }
}

import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { NutrientApiService, FertilizerProductDto } from '../services/nutrient-api.service';

@Component({
  templateUrl: './fertilizer-products.component.html',
  animations: [appModuleAnimation()],
})
export class FertilizerProductsComponent extends AppComponentBase implements OnInit {
  products: FertilizerProductDto[] = [];
  loading = true;
  canApply = false;

  newProduct: { name: string; nitrogenPercent?: number; phosphorusPercent?: number; potassiumPercent?: number; unitCostPerKg?: number } = { name: '' };

  constructor(injector: Injector, private nutrientApi: NutrientApiService, private cd: ChangeDetectorRef) {
    super(injector);
  }

  ngOnInit(): void {
    this.canApply = this.permission.isGranted('Pages.Nutrients.Apply');
    this.load();
  }

  load(): void {
    this.loading = true;
    this.nutrientApi.getProducts().subscribe({
      next: (r) => {
        this.products = r.items;
        this.loading = false;
        this.cd.detectChanges();
      },
      error: () => {
        this.products = [];
        this.loading = false;
        this.cd.detectChanges();
      },
    });
  }

  create(): void {
    if (
      !this.newProduct.name ||
      this.newProduct.nitrogenPercent == null ||
      this.newProduct.phosphorusPercent == null ||
      this.newProduct.potassiumPercent == null
    ) {
      return;
    }

    this.nutrientApi
      .createProduct({
        name: this.newProduct.name,
        nitrogenPercent: this.newProduct.nitrogenPercent,
        phosphorusPercent: this.newProduct.phosphorusPercent,
        potassiumPercent: this.newProduct.potassiumPercent,
        unitCostPerKg: this.newProduct.unitCostPerKg,
      })
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.newProduct = { name: '' };
        this.load();
      });
  }
}

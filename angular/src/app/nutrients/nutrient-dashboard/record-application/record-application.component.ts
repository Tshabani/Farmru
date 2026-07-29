import { ChangeDetectorRef, Component, Injector, OnInit, output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { NutrientApiService, FertilizerProductDto } from '../../services/nutrient-api.service';

@Component({
  templateUrl: './record-application.component.html',
})
export class RecordApplicationComponent extends AppComponentBase implements OnInit {
  fieldId: string;
  saving = false;
  products: FertilizerProductDto[] = [];

  input: { productId?: string; rateKgPerHectare?: number; applicationDate?: string; cost?: number } = {};

  onSave = output<void>();

  constructor(injector: Injector, private nutrientApi: NutrientApiService, public bsModalRef: BsModalRef, private cd: ChangeDetectorRef) {
    super(injector);
  }

  ngOnInit(): void {
    this.nutrientApi.getProducts().subscribe((r) => {
      this.products = r.items;
      this.cd.detectChanges();
    });
  }

  save(): void {
    if (!this.input.productId || !this.input.rateKgPerHectare || !this.input.applicationDate) {
      return;
    }
    this.saving = true;
    this.nutrientApi
      .recordApplication({
        fieldId: this.fieldId,
        productId: this.input.productId,
        rateKgPerHectare: this.input.rateKgPerHectare,
        applicationDate: this.input.applicationDate,
        cost: this.input.cost,
      })
      .subscribe({
        next: () => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => {
          this.saving = false;
          this.cd.detectChanges();
        },
      });
  }
}

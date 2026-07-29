import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { ModalModule } from 'ngx-bootstrap/modal';
import { CropsRoutingModule } from './crops-routing.module';
import { CropSeasonListComponent } from './crop-season-list/crop-season-list.component';
import { CropSeasonDetailComponent } from './crop-season-detail/crop-season-detail.component';
import { PlantCropSeasonComponent } from './crop-season-list/plant-crop-season/plant-crop-season.component';
import { LogGrowthStageComponent } from './crop-season-detail/log-growth-stage/log-growth-stage.component';
import { HarvestSeasonComponent } from './crop-season-detail/harvest-season/harvest-season.component';
import { FieldListComponent } from './field-list/field-list.component';
import { CreateEditFieldComponent } from './field-list/create-edit-field/create-edit-field.component';
import { CropReferenceDataComponent } from './crop-reference-data/crop-reference-data.component';

@NgModule({
  declarations: [
    CropSeasonListComponent,
    CropSeasonDetailComponent,
    PlantCropSeasonComponent,
    LogGrowthStageComponent,
    HarvestSeasonComponent,
    FieldListComponent,
    CreateEditFieldComponent,
    CropReferenceDataComponent,
  ],
  imports: [CommonModule, SharedModule, ServiceProxyModule, CropsRoutingModule, FormsModule, ModalModule.forChild()],
})
export class CropsModule {}

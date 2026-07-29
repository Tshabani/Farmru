import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { CropsRoutingModule } from './crops-routing.module';
import { CropSeasonListComponent } from './crop-season-list/crop-season-list.component';
import { CropSeasonDetailComponent } from './crop-season-detail/crop-season-detail.component';

@NgModule({
  declarations: [CropSeasonListComponent, CropSeasonDetailComponent],
  imports: [CommonModule, SharedModule, ServiceProxyModule, CropsRoutingModule, FormsModule],
})
export class CropsModule {}

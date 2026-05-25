import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { GeoApiService, ExecutiveGisSummaryDto } from '../services/geo-api.service';

@Component({ templateUrl: './executive-gis.component.html' })
export class ExecutiveGisComponent extends AppComponentBase implements OnInit {
  summary: ExecutiveGisSummaryDto;
  loading = true;

  constructor(
    injector: Injector,
    private geoApi: GeoApiService,
    private router: Router,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.geoApi.getExecutiveSummary().subscribe((s) => {
      this.summary = s;
      this.loading = false;
      this.cd.detectChanges();
    });
  }

  openMap(): void {
    this.router.navigate(['/app/gis']);
  }
}

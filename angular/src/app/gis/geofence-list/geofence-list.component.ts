import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { GeoApiService, GeoFenceDto } from '../services/geo-api.service';

@Component({ templateUrl: './geofence-list.component.html' })
export class GeofenceListComponent extends AppComponentBase implements OnInit {
  fences: GeoFenceDto[] = [];
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
    this.load();
  }

  load(): void {
    this.loading = true;
    this.geoApi.getGeoFences().subscribe((f) => {
      this.fences = f;
      this.loading = false;
      this.cd.detectChanges();
    });
  }

  create(): void {
    this.router.navigate(['/app/gis/geofences/create']);
  }

  edit(f: GeoFenceDto): void {
    this.router.navigate(['/app/gis/geofences', f.id]);
  }

  remove(f: GeoFenceDto): void {
    this.message.confirm('Delete this geo-fence?', '', (ok) => {
      if (ok) {
        this.geoApi.deleteGeoFence(f.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.load();
        });
      }
    });
  }

  back(): void {
    this.router.navigate(['/app/gis']);
  }
}

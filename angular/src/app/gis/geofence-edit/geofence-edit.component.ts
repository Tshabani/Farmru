import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { GeoApiService, GeoFenceDto } from '../services/geo-api.service';

@Component({ templateUrl: './geofence-edit.component.html' })
export class GeofenceEditComponent extends AppComponentBase implements OnInit {
  model: GeoFenceDto = {
    name: '',
    geoFenceType: 0,
    severity: 1,
    isActive: true,
    triggerAlertOnEntry: true,
    triggerAlertOnExit: true,
    centerLatitude: -25.27,
    centerLongitude: 133.78,
    radiusMeters: 500,
    polygonJson: '[{"lat":-25.27,"lng":133.77},{"lat":-25.26,"lng":133.79},{"lat":-25.28,"lng":133.80}]',
  };
  isEdit = false;
  loading = false;

  constructor(
    injector: Injector,
    private route: ActivatedRoute,
    private router: Router,
    private geoApi: GeoApiService,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id && id !== 'create') {
      this.isEdit = true;
      this.geoApi.getGeoFences().subscribe((list) => {
        const found = list.find((x) => x.id === id);
        if (found) {
          this.model = { ...found };
        }
        this.cd.detectChanges();
      });
    }
  }

  save(): void {
    this.loading = true;
    const req = this.isEdit
      ? this.geoApi.updateGeoFence({ ...this.model, id: this.model.id })
      : this.geoApi.createGeoFence(this.model);
    req.subscribe(() => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.router.navigate(['/app/gis/geofences']);
    });
  }

  back(): void {
    this.router.navigate(['/app/gis/geofences']);
  }
}

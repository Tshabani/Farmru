import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  Injector,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { Router } from '@angular/router';
import * as L from 'leaflet';
import 'leaflet.markercluster';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { GeoApiService, OperationalMapDto } from '../services/geo-api.service';
import { AlertSignalRService } from '../../alerts/services/alert-signalr.service';
import { Subscription } from 'rxjs';

@Component({
  templateUrl: './operational-map.component.html',
  animations: [appModuleAnimation()],
})
export class OperationalMapComponent extends AppComponentBase implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('mapContainer', { static: true }) mapContainer: ElementRef<HTMLDivElement>;

  mapData: OperationalMapDto;
  loading = true;
  feed: { type: string; message: string; time: string }[] = [];
  private map: L.Map;
  private cluster: L.MarkerClusterGroup;
  private subs: Subscription[] = [];

  constructor(
    injector: Injector,
    private geoApi: GeoApiService,
    private alertSignalR: AlertSignalRService,
    private router: Router,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.alertSignalR.start();
    this.subs.push(
      this.alertSignalR.mapUpdate$.subscribe((e) => this.onMapEvent(e?.eventType ?? 'map', e?.message ?? '')),
      this.alertSignalR.alertChanged$.subscribe((e) => this.onMapEvent(e.action, e.alert?.title ?? ''))
    );
    this.load();
  }

  ngAfterViewInit(): void {
    this.initMap();
  }

  ngOnDestroy(): void {
    this.subs.forEach((s) => s.unsubscribe());
    this.map?.remove();
  }

  load(): void {
    this.loading = true;
    this.geoApi.getOperationalMap().subscribe((data) => {
      this.mapData = data;
      this.loading = false;
      this.renderLayers();
      this.cd.detectChanges();
    });
  }

  openExecutive(): void {
    this.router.navigate(['/app/gis/executive']);
  }

  openGeoFences(): void {
    this.router.navigate(['/app/gis/geofences']);
  }

  openDevice(id: string): void {
    this.router.navigate(['/app/node/device', id]);
  }

  private initMap(): void {
    if (this.map) {
      return;
    }
    this.map = L.map(this.mapContainer.nativeElement).setView([-25.27, 133.78], 4);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap',
    }).addTo(this.map);
    this.cluster = L.markerClusterGroup();
    this.map.addLayer(this.cluster);
  }

  private renderLayers(): void {
    if (!this.map || !this.mapData) {
      return;
    }
    this.cluster.clearLayers();

    this.mapData.facilities?.forEach((f) => {
      const marker = L.circleMarker([f.latitude, f.longitude], {
        radius: 10,
        color: f.operationalScore < 50 ? '#dc3545' : '#28a745',
        fillOpacity: 0.7,
      }).bindPopup(
        `<strong>${f.name}</strong><br/>Devices: ${f.deviceCount}<br/>Alerts: ${f.activeAlertCount}<br/>Score: ${f.operationalScore}`
      );
      this.cluster.addLayer(marker);
    });

    this.mapData.devices?.forEach((d) => {
      const color = d.isOnline ? (d.healthStatus >= 2 ? '#ffc107' : '#007bff') : '#dc3545';
      const marker = L.circleMarker([d.latitude, d.longitude], { radius: 8, color, fillOpacity: 0.85 }).bindPopup(
        `<strong>${d.displayText}</strong><br/>${d.isOnline ? 'Online' : 'Offline'}<br/>Alerts: ${d.activeAlertCount}<br/><a href="#" data-id="${d.id}" class="device-link">Details</a>`
      );
      marker.on('popupopen', () => {
        const el = document.querySelector('.device-link');
        el?.addEventListener('click', (ev) => {
          ev.preventDefault();
          this.openDevice(d.id);
        });
      });
      this.cluster.addLayer(marker);
    });

    this.mapData.alerts?.forEach((a) => {
      const marker = L.marker([a.latitude, a.longitude], {
        icon: L.divIcon({
          className: 'alert-pin',
          html: `<span style="background:${a.severity === 2 ? '#dc3545' : '#ffc107'};color:#fff;padding:2px 6px;border-radius:4px;font-size:10px">!</span>`,
        }),
      }).bindPopup(`<strong>${a.title}</strong>`);
      this.cluster.addLayer(marker);
    });

    this.mapData.geoFences?.forEach((g) => {
      if (g.geoFenceType === 0 && g.centerLatitude != null && g.radiusMeters) {
        L.circle([g.centerLatitude, g.centerLongitude], { radius: g.radiusMeters, color: '#6f42c1', fillOpacity: 0.1 }).addTo(
          this.map
        );
      } else if (g.polygonJson) {
        try {
          const pts = JSON.parse(g.polygonJson).map((p: any) => [p.lat ?? p.latitude, p.lng ?? p.longitude]);
          if (pts.length >= 3) {
            L.polygon(pts, { color: '#6f42c1', fillOpacity: 0.1 }).addTo(this.map);
          }
        } catch {
          /* ignore invalid polygon */
        }
      }
    });

    if (this.mapData.devices?.length) {
      const bounds = L.latLngBounds(this.mapData.devices.map((d) => [d.latitude, d.longitude]));
      this.map.fitBounds(bounds.pad(0.2));
    }
  }

  private onMapEvent(type: string, message: string): void {
    this.feed.unshift({ type, message, time: new Date().toISOString() });
    if (this.feed.length > 30) {
      this.feed.pop();
    }
    this.load();
  }
}

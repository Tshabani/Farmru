import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '@shared/service-proxies/service-proxies';

export interface OperationalMapDto {
  devices: DeviceMapMarkerDto[];
  facilities: FacilityMapMarkerDto[];
  alerts: AlertMapMarkerDto[];
  incidents: IncidentMapMarkerDto[];
  geoFences: GeoFenceMapDto[];
}

export interface DeviceMapMarkerDto {
  id: string;
  displayText: string;
  latitude: number;
  longitude: number;
  isOnline: boolean;
  healthStatus: number;
  batteryLevel?: number;
  facilityId?: string;
  facilityName?: string;
  activeAlertCount: number;
  latestAlertTitle?: string;
}

export interface FacilityMapMarkerDto {
  id: string;
  name: string;
  latitude: number;
  longitude: number;
  deviceCount: number;
  activeAlertCount: number;
  offlineDeviceCount: number;
  operationalScore: number;
}

export interface AlertMapMarkerDto {
  id: string;
  title: string;
  severity: number;
  alertType: number;
  latitude: number;
  longitude: number;
  nodeId?: string;
}

export interface IncidentMapMarkerDto {
  id: string;
  title: string;
  priority: number;
  status: number;
  latitude: number;
  longitude: number;
}

export interface GeoFenceMapDto {
  id: string;
  name: string;
  geoFenceType: number;
  centerLatitude?: number;
  centerLongitude?: number;
  radiusMeters?: number;
  polygonJson?: string;
  isActive: boolean;
  severity: number;
  facilityId?: string;
}

export interface ExecutiveGisSummaryDto {
  totalDevices: number;
  onlineDevices: number;
  offlineDevices: number;
  criticalAlerts: number;
  activeGeoFences: number;
  facilitiesMonitored: number;
  heatmap: { latitude: number; longitude: number; weight: number; severity: number }[];
  topRiskFacilities: FacilityMapMarkerDto[];
}

export interface GeoFenceDto {
  id?: string;
  name: string;
  description?: string;
  geoFenceType: number;
  centerLatitude?: number;
  centerLongitude?: number;
  radiusMeters?: number;
  polygonJson?: string;
  isActive: boolean;
  severity: number;
  facilityId?: string;
  triggerAlertOnExit: boolean;
  triggerAlertOnEntry: boolean;
}

@Injectable({ providedIn: 'root' })
export class GeoApiService {
  private baseUrl: string;

  constructor(private http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
    this.baseUrl = baseUrl ?? '';
  }

  getOperationalMap(): Observable<OperationalMapDto> {
    return this.http.get<OperationalMapDto>(`${this.baseUrl}/api/services/app/GeoSpatial/GetOperationalMap`);
  }

  getExecutiveSummary(): Observable<ExecutiveGisSummaryDto> {
    return this.http.get<ExecutiveGisSummaryDto>(`${this.baseUrl}/api/services/app/GeoSpatial/GetExecutiveSummary`);
  }

  getGeoFences(): Observable<GeoFenceDto[]> {
    return this.http.get<GeoFenceDto[]>(`${this.baseUrl}/api/services/app/GeoSpatial/GetGeoFences`);
  }

  createGeoFence(input: GeoFenceDto): Observable<GeoFenceDto> {
    return this.http.post<GeoFenceDto>(`${this.baseUrl}/api/services/app/GeoSpatial/CreateGeoFence`, input);
  }

  updateGeoFence(input: GeoFenceDto & { id: string }): Observable<GeoFenceDto> {
    return this.http.post<GeoFenceDto>(`${this.baseUrl}/api/services/app/GeoSpatial/UpdateGeoFence`, input);
  }

  deleteGeoFence(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/api/services/app/GeoSpatial/DeleteGeoFence`, { params: { Id: id } });
  }
}

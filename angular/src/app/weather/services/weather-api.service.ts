import { Inject, Injectable, Optional } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '@shared/service-proxies/service-proxies';

// Hand-written, matching the existing AlertApiService pattern (see
// app/alerts/services/alert-api.service.ts) rather than NSwag-generated
// proxies — see Phase 1 Technical Design Section 9 / commit history: NSwag
// cannot regenerate against these new backend endpoints without a live API
// instance, and this codebase already has a non-NSwag HttpClient convention
// for exactly this situation.

export interface WeatherObservationDto {
  id: string;
  facilityId: string;
  observedAt: string;
  temperatureCelsius: number;
  humidityPercent: number;
  windSpeedKph?: number;
  precipitationMm?: number;
  pressureHpa?: number;
  uvIndex?: number;
  lightningProbabilityPercent?: number;
}

export interface WeatherForecastDto {
  id: string;
  facilityId: string;
  forecastFor: string;
  generatedAt: string;
  tempMinCelsius: number;
  tempMaxCelsius: number;
  precipitationProbabilityPercent: number;
  windGustKph?: number;
  frostRisk: number;
  heatStress: number;
}

export interface EvapotranspirationDto {
  id: string;
  facilityId: string;
  date: string;
  et0Mm: number;
  etcMm?: number;
}

export interface WeatherAlertRuleDto {
  id: string;
  facility?: { id: string; displayText: string };
  organisation?: { id: string; displayText: string };
  alertType: number;
  thresholdValue: number;
  severity: number;
  isActive: boolean;
}

@Injectable({ providedIn: 'root' })
export class WeatherApiService {
  private baseUrl: string;

  constructor(private http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
    this.baseUrl = baseUrl ?? '';
  }

  getCurrent(facilityId: string): Observable<WeatherObservationDto> {
    return this.http.get<WeatherObservationDto>(`${this.baseUrl}/api/services/app/Weather/GetCurrent`, {
      params: { Id: facilityId },
    });
  }

  getForecast(facilityId: string): Observable<WeatherForecastDto[]> {
    return this.http.get<WeatherForecastDto[]>(`${this.baseUrl}/api/services/app/Weather/GetForecast`, {
      params: { Id: facilityId },
    });
  }

  getHistory(facilityId: string, skipCount = 0, maxResultCount = 30): Observable<{ totalCount: number; items: WeatherObservationDto[] }> {
    const params = new HttpParams()
      .set('FacilityId', facilityId)
      .set('SkipCount', skipCount)
      .set('MaxResultCount', maxResultCount);
    return this.http.get<{ totalCount: number; items: WeatherObservationDto[] }>(
      `${this.baseUrl}/api/services/app/Weather/GetHistory`,
      { params }
    );
  }

  getEvapotranspiration(facilityId: string): Observable<EvapotranspirationDto[]> {
    return this.http.get<EvapotranspirationDto[]>(`${this.baseUrl}/api/services/app/Weather/GetEvapotranspiration`, {
      params: { FacilityId: facilityId },
    });
  }

  getAlertRulesForFacility(facilityId: string): Observable<WeatherAlertRuleDto[]> {
    return this.http.get<WeatherAlertRuleDto[]>(`${this.baseUrl}/api/services/app/WeatherAlertRule/GetForFacility`, {
      params: { Id: facilityId },
    });
  }

  createAlertRule(input: {
    facilityId?: string;
    organisationId?: string;
    alertType: number;
    thresholdValue: number;
    severity: number;
  }): Observable<WeatherAlertRuleDto> {
    return this.http.post<WeatherAlertRuleDto>(`${this.baseUrl}/api/services/app/WeatherAlertRule/Create`, input);
  }

  deactivateAlertRule(id: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/api/services/app/WeatherAlertRule/Deactivate`, { id });
  }
}

import { Inject, Injectable, Optional } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '@shared/service-proxies/service-proxies';
import { DisplayRefDto } from '../../crops/services/crop-api.service';

// Hand-written, matching the existing AlertApiService pattern — see
// app/weather/services/weather-api.service.ts for the rationale.

export interface FertilizerProductDto {
  id: string;
  name: string;
  nitrogenPercent: number;
  phosphorusPercent: number;
  potassiumPercent: number;
  unitCostPerKg?: number;
}

export interface FertilizerApplicationDto {
  id: string;
  field?: DisplayRefDto;
  cropSeason?: DisplayRefDto;
  product?: DisplayRefDto;
  rateKgPerHectare: number;
  applicationDate: string;
  cost?: number;
  operator?: DisplayRefDto;
}

export interface NutrientBalanceSnapshotDto {
  id: string;
  fieldId: string;
  snapshotDate: string;
  sensedNitrogen: number;
  sensedPhosphorus: number;
  sensedPotassium: number;
  appliedNitrogenTrailing30d: number;
  appliedPhosphorusTrailing30d: number;
  appliedPotassiumTrailing30d: number;
  nitrogenStatus: number; // 0=Deficient 1=Adequate 2=Surplus
  phosphorusStatus: number;
  potassiumStatus: number;
}

@Injectable({ providedIn: 'root' })
export class NutrientApiService {
  private baseUrl: string;

  constructor(private http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
    this.baseUrl = baseUrl ?? '';
  }

  getProducts(): Observable<{ totalCount: number; items: FertilizerProductDto[] }> {
    return this.http.get<{ totalCount: number; items: FertilizerProductDto[] }>(
      `${this.baseUrl}/api/services/app/Fertilizer/GetProducts`,
      { params: { SkipCount: 0, MaxResultCount: 200 } }
    );
  }

  createProduct(input: {
    name: string;
    nitrogenPercent: number;
    phosphorusPercent: number;
    potassiumPercent: number;
    unitCostPerKg?: number;
  }): Observable<FertilizerProductDto> {
    return this.http.post<FertilizerProductDto>(`${this.baseUrl}/api/services/app/Fertilizer/CreateProduct`, input);
  }

  recordApplication(input: {
    fieldId: string;
    cropSeasonId?: string;
    productId: string;
    rateKgPerHectare: number;
    applicationDate: string;
    cost?: number;
    operatorPersonId?: string;
  }): Observable<FertilizerApplicationDto> {
    return this.http.post<FertilizerApplicationDto>(`${this.baseUrl}/api/services/app/Fertilizer/RecordApplication`, input);
  }

  getApplicationsByField(fieldId: string, skipCount = 0, maxResultCount = 50): Observable<{ totalCount: number; items: FertilizerApplicationDto[] }> {
    const params = new HttpParams()
      .set('FieldId', fieldId)
      .set('SkipCount', skipCount)
      .set('MaxResultCount', maxResultCount);
    return this.http.get<{ totalCount: number; items: FertilizerApplicationDto[] }>(
      `${this.baseUrl}/api/services/app/Fertilizer/GetApplicationsByField`,
      { params }
    );
  }

  getLatestBalance(fieldId: string): Observable<NutrientBalanceSnapshotDto> {
    return this.http.get<NutrientBalanceSnapshotDto>(`${this.baseUrl}/api/services/app/NutrientBalance/GetLatest`, {
      params: { Id: fieldId },
    });
  }

  getBalanceHistory(fieldId: string): Observable<NutrientBalanceSnapshotDto[]> {
    return this.http.get<NutrientBalanceSnapshotDto[]>(`${this.baseUrl}/api/services/app/NutrientBalance/GetHistory`, {
      params: { FieldId: fieldId },
    });
  }
}

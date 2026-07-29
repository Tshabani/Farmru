import { Inject, Injectable, Optional } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '@shared/service-proxies/service-proxies';

// Hand-written, matching the existing AlertApiService pattern — see
// app/weather/services/weather-api.service.ts for the rationale.

export interface DisplayRefDto {
  id: string;
  displayText: string;
}

export interface FieldDto {
  id: string;
  facility?: DisplayRefDto;
  name: string;
  areaHectares?: number;
  soilType?: string;
  boundaryGeoFenceId?: string;
}

export interface CropTypeDto {
  id: string;
  name: string;
  scientificName?: string;
  typicalGrowthDurationDays: number;
  isActive: boolean;
}

export interface SeedVarietyDto {
  id: string;
  cropType?: DisplayRefDto;
  supplier?: DisplayRefDto;
  name: string;
  daysToMaturity?: number;
}

export interface CropSeasonDto {
  id: string;
  field?: DisplayRefDto;
  cropType?: DisplayRefDto;
  seedVariety?: DisplayRefDto;
  plantingDate: string;
  expectedHarvestDate: string;
  expectedYieldKg?: number;
  plantPopulationPerHectare?: number;
  status: number; // 0=Planned 1=Growing 2=Harvested 3=Closed
}

export interface GrowthStageEventDto {
  id: string;
  stage: number;
  observedDate: string;
  source: number;
}

export interface HarvestRecordDto {
  id: string;
  harvestDate: string;
  actualYieldKg: number;
  qualityGrade?: string;
}

export interface CropSeasonDetailDto extends CropSeasonDto {
  stageEvents: GrowthStageEventDto[];
  harvest?: HarvestRecordDto;
}

export interface CropRotationEntryDto {
  cropSeasonId: string;
  cropTypeName: string;
  plantingDate: string;
  harvestDate?: string;
  actualYieldKg?: number;
}

@Injectable({ providedIn: 'root' })
export class CropApiService {
  private baseUrl: string;

  constructor(private http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
    this.baseUrl = baseUrl ?? '';
  }

  // Fields
  getFields(skipCount = 0, maxResultCount = 200): Observable<{ totalCount: number; items: FieldDto[] }> {
    const params = new HttpParams().set('SkipCount', skipCount).set('MaxResultCount', maxResultCount);
    return this.http.get<{ totalCount: number; items: FieldDto[] }>(`${this.baseUrl}/api/services/app/Field/GetAll`, { params });
  }

  getFieldsByFacility(facilityId: string): Observable<FieldDto[]> {
    return this.http.get<FieldDto[]>(`${this.baseUrl}/api/services/app/Field/GetByFacility`, {
      params: { FacilityId: facilityId },
    });
  }

  createField(input: { facilityId: string; name: string; areaHectares?: number; soilType?: string }): Observable<FieldDto> {
    return this.http.post<FieldDto>(`${this.baseUrl}/api/services/app/Field/Create`, input);
  }

  // Crop reference data
  getCropTypes(): Observable<{ totalCount: number; items: CropTypeDto[] }> {
    return this.http.get<{ totalCount: number; items: CropTypeDto[] }>(`${this.baseUrl}/api/services/app/CropType/GetAll`, {
      params: { SkipCount: 0, MaxResultCount: 200 },
    });
  }

  createCropType(input: { name: string; scientificName?: string; typicalGrowthDurationDays: number }): Observable<CropTypeDto> {
    return this.http.post<CropTypeDto>(`${this.baseUrl}/api/services/app/CropType/Create`, input);
  }

  getSeedVarieties(): Observable<{ totalCount: number; items: SeedVarietyDto[] }> {
    return this.http.get<{ totalCount: number; items: SeedVarietyDto[] }>(`${this.baseUrl}/api/services/app/SeedVariety/GetAll`, {
      params: { SkipCount: 0, MaxResultCount: 200 },
    });
  }

  // Crop seasons
  getSeasonsByField(fieldId: string, skipCount = 0, maxResultCount = 50): Observable<{ totalCount: number; items: CropSeasonDto[] }> {
    const params = new HttpParams()
      .set('FieldId', fieldId)
      .set('SkipCount', skipCount)
      .set('MaxResultCount', maxResultCount);
    return this.http.get<{ totalCount: number; items: CropSeasonDto[] }>(`${this.baseUrl}/api/services/app/CropSeason/GetByField`, {
      params,
    });
  }

  getSeasonDetail(id: string): Observable<CropSeasonDetailDto> {
    return this.http.get<CropSeasonDetailDto>(`${this.baseUrl}/api/services/app/CropSeason/GetDetail`, {
      params: { Id: id },
    });
  }

  plantSeason(input: {
    fieldId: string;
    cropTypeId: string;
    seedVarietyId?: string;
    plantingDate: string;
    expectedHarvestDate: string;
    expectedYieldKg?: number;
    plantPopulationPerHectare?: number;
  }): Observable<CropSeasonDto> {
    return this.http.post<CropSeasonDto>(`${this.baseUrl}/api/services/app/CropSeason/Plant`, input);
  }

  logGrowthStage(input: { cropSeasonId: string; stage: number; observedDate: string }): Observable<CropSeasonDetailDto> {
    return this.http.post<CropSeasonDetailDto>(`${this.baseUrl}/api/services/app/CropSeason/LogGrowthStage`, input);
  }

  harvestSeason(input: {
    cropSeasonId: string;
    harvestDate: string;
    actualYieldKg: number;
    qualityGrade?: string;
  }): Observable<CropSeasonDetailDto> {
    return this.http.post<CropSeasonDetailDto>(`${this.baseUrl}/api/services/app/CropSeason/Harvest`, input);
  }

  closeSeason(id: string): Observable<CropSeasonDto> {
    return this.http.post<CropSeasonDto>(`${this.baseUrl}/api/services/app/CropSeason/Close`, { id });
  }

  getRotationHistory(fieldId: string): Observable<CropRotationEntryDto[]> {
    return this.http.get<CropRotationEntryDto[]>(`${this.baseUrl}/api/services/app/CropSeason/GetRotationHistory`, {
      params: { Id: fieldId },
    });
  }
}

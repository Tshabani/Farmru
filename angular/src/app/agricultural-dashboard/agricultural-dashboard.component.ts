import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { CurrentReadings, HistoricalData, HistoricalDataResponse } from '@shared/service-proxies/service-proxies';
import { ChartType } from 'angular-google-charts';

@Component({
  selector: 'app-agricultural-dashboard',
  templateUrl: './agricultural-dashboard.component.html',
  styleUrls: ['./agricultural-dashboard.component.scss']
})
export class AgriculturalDashboardComponent implements OnChanges {
  @Input() readingsData: HistoricalDataResponse; 
  
  // Current readings
  currentReadings: CurrentReadings;

  // Sample historical data (would be replaced with actual data)
  historicalData: HistoricalData[];
  
  // Nutrient levels status and recommendations - these will be initialized in ngOnInit
  nutrientStatus: {
    [key: string]: { status: string, action: string }
  } = {};

  // System alerts based on current values - these will be initialized in ngOnInit
  systemAlerts: Array<{ level: string, message: string, type: string }> = [];

  // Google Charts configurations
  chartType = ChartType.Gauge;
  soilTemperatureGauge: any;
  soilPHGauge: any;
  moistureGauge: any;
  conductivityGauge: any;
  phosphorusGauge: any;
  potassiumGauge: any;
  nitrogenGauge: any;
  solarPanelVoltageGauge: any;
  batteryVoltageGauge: any;
  
  // Line chart for historical data
  lineChartType = ChartType.LineChart;
  temperatureChart: any;
  pHChart: any;
  moistureChart: any;

  // Recommendations - these will be initialized in ngOnInit
  recommendations: Array<{ priority: number, type: string, text: string }> = [];

  getSystemStatus(key: string): string {
    const value = this.currentReadings[key];
    if (value === 0) return 'Critical';
    return 'Normal';
  }

  // NEW METHOD: Get human-friendly title for each metric
  getMetricTitle(metric: string): string {
    const titles = {
      soilTemp: 'Soil Temperature',
      soilPH: 'Soil pH',
      moisture: 'Soil Moisture',
      phosphorus: 'Phosphorus Level',
      potassium: 'Potassium Level',
      nitrogen: 'Nitrogen Level',
      solarVoltage: 'Solar Panel Voltage',
      batteryVoltage: 'Battery Voltage',
      soilConductivity: 'Soil Conductivity'
    };
    return titles[metric] || metric;
  }

  // NEW METHOD: Get appropriate unit for each metric
  getUnit(metric: string): string {
    const units = {
      soilTemp: '°C',
      soilPH: '',
      moisture: '%',
      phosphorus: ' ppm',
      potassium: ' ppm',
      nitrogen: ' ppm',
      solarVoltage: 'V',
      batteryVoltage: 'V',
      soilConductivity: 'S/m'
    };
    return units[metric] || '';
  }

  // NEW METHOD: Get appropriate chart for each metric
  getChart(metric: string): any { 
    const chartMap = {
      soilTemp: this.temperatureChart,
      soilPH: this.pHChart,
      moisture: this.moistureChart,
      phosphorus: this.phosphorusGauge,
      potassium: this.potassiumGauge,
      nitrogen: this.nitrogenGauge,
      solarVoltage: this.solarPanelVoltageGauge,
      batteryVoltage: this.batteryVoltageGauge,      
      soilConductivity: this.conductivityGauge
    };
    return chartMap[metric] || null;
  }

  getGauge(key: string) {
    switch (key) {
      case 'pH': return this.soilPHGauge;
      case 'nitrogen': return this.nitrogenGauge;
      case 'phosphorus': return this.phosphorusGauge;
      case 'potassium': return this.potassiumGauge;
      case 'solarPanelVoltage': return this.solarPanelVoltageGauge;
      case 'batteryVoltage': return this.batteryVoltageGauge;
      case 'conductivity': return this.conductivityGauge;
      default: return null;
    }
  }

  getAlertClass(status: string): string {
    switch (status) {
      case 'Critical': case 'Very Acidic': return 'bg-danger-subtle';
      case 'Warning': return 'bg-warning-subtle';
      case 'Normal': return 'bg-info-subtle';
      default: return 'bg-light';
    }
  }

  getBadgeClass(status: string): string {
    switch (status) {
      case 'Critical': case 'Very Acidic': return 'bg-danger';
      case 'Warning': return 'bg-warning text-dark';
      case 'High': return 'bg-danger';
      case 'Low': return 'bg-warning text-dark';
      case 'Normal': return 'bg-success';
      default: return 'bg-secondary';
    }
  }

  getTextClass(status: string): string {
    switch (status) {
      case 'Critical': case 'Very Acidic': return 'text-danger';
      case 'Warning': return 'text-warning';
      case 'Normal': return 'text-info';
      default: return 'text-dark';
    }
  }

  constructor() { }
 

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['readingsData'] && this.readingsData) { 
      this.currentReadings = this.readingsData.currentReadings;
      this.historicalData = this.readingsData.historicalData;

      // First calculate and update all statuses based on initial data
      this.updateNutrientStatus();
      this.updateAlerts(); 

       // Then initialize charts with the updated status information
      this.initializeGaugeCharts();
      this.initializeLineCharts(); 
    }
  }

  updateAlerts(): void {
    this.systemAlerts = [];
  
    // Check soil pH
    if (this.currentReadings?.soilPH < 5.5) {
      this.systemAlerts.push({
        level: 'Critical',
        message: `Soil pH critically low (${this.currentReadings?.soilPH}) - Add agricultural lime or dolomite to raise pH.`,
        type: 'soil'
      });
    } else if (this.currentReadings?.soilPH >= 5.5 && this.currentReadings?.soilPH < 6.0) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Soil pH slightly low (${this.currentReadings?.soilPH}) - Consider adding lime or organic matter to improve pH.`,
        type: 'soil'
      });
    } else if (this.currentReadings?.soilPH > 7.5) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Soil pH too high (${this.currentReadings?.soilPH}) - Consider adding sulfur or acidifying fertilizers to lower pH.`,
        type: 'soil'
      });
    }
  
    // Check soil temperature
    if (this.currentReadings?.soilTemp > 35) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Soil temperature too high (${this.currentReadings?.soilTemp}°C) - Use mulch or shade to cool the soil and protect plants.`,
        type: 'soil'
      });
    } else if (this.currentReadings?.soilTemp < 10) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Soil temperature too low (${this.currentReadings?.soilTemp}°C) - Consider using row covers or black plastic to warm the soil.`,
        type: 'soil'
      });
    }
  
    // Check moisture level
    if (this.currentReadings?.moisture > 90) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Moisture level too high (${this.currentReadings?.moisture}%) - Improve drainage or reduce irrigation to prevent waterlogging.`,
        type: 'soil'
      });
    } else if (this.currentReadings?.moisture < 30) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Moisture level too low (${this.currentReadings?.moisture}%) - Increase irrigation or use mulch to retain soil moisture.`,
        type: 'soil'
      });
    }
  
    // Check phosphorus level
    if (this.currentReadings?.phosphorus < 20) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Phosphorus level low (${this.currentReadings?.phosphorus}ppm) - Apply bone meal or rock phosphate to increase phosphorus.`,
        type: 'soil'
      });
    } else if (this.currentReadings?.phosphorus > 100) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Phosphorus level high (${this.currentReadings?.phosphorus}ppm) - Avoid phosphorus-rich fertilizers to prevent nutrient imbalance.`,
        type: 'soil'
      });
    }
  
    // Check potassium level
    if (this.currentReadings?.potassium < 150) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Potassium level low (${this.currentReadings?.potassium}ppm) - Apply potash or wood ash to increase potassium.`,
        type: 'soil'
      });
    } else if (this.currentReadings?.potassium > 300) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Potassium level high (${this.currentReadings?.potassium}ppm) - Avoid potassium-rich fertilizers to prevent nutrient imbalance.`,
        type: 'soil'
      });
    }
  
    // Check nitrogen level
    if (this.currentReadings?.nitrogen < 25) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Nitrogen level low (${this.currentReadings?.nitrogen}ppm) - Apply compost, manure, or nitrogen-rich fertilizers.`,
        type: 'soil'
      });
    } else if (this.currentReadings?.nitrogen > 50) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Nitrogen level high (${this.currentReadings?.nitrogen}ppm) - Avoid nitrogen-rich fertilizers to prevent overgrowth and nutrient runoff.`,
        type: 'soil'
      });
    }
  
    // Check solar panel voltage
    if (this.currentReadings?.solarVoltage < 11.0) {
      this.systemAlerts.push({
        level: 'Critical',
        message: `Solar panel voltage critically low (${this.currentReadings?.solarVoltage}V) - Check for obstructions, damage, or disconnections.`,
        type: 'system'
      });
    } else if (this.currentReadings?.solarVoltage >= 11.0 && this.currentReadings?.solarVoltage < 12.0) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Solar panel voltage low (${this.currentReadings?.solarVoltage}V) - Clean the panels or check for shading issues.`,
        type: 'system'
      });
    } else if (this.currentReadings?.solarVoltage >= 12.0 && this.currentReadings?.solarVoltage < 13.0) {
      this.systemAlerts.push({
        level: 'Info',
        message: `Solar panel voltage slightly low (${this.currentReadings?.solarVoltage}V) - Monitor and ensure panels are clean and unobstructed.`,
        type: 'system'
      });
    }
  
    // Check battery voltage
    if (this.currentReadings?.batteryVoltage < 10.5) {
      this.systemAlerts.push({
        level: 'Critical',
        message: `Battery voltage critically low (${this.currentReadings?.batteryVoltage}V) - Recharge or replace the battery immediately to avoid damage.`,
        type: 'system'
      });
    } else if (this.currentReadings?.batteryVoltage >= 10.5 && this.currentReadings?.batteryVoltage < 11.5) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Battery voltage low (${this.currentReadings?.batteryVoltage}V) - Recharge the battery soon to avoid system shutdown.`,
        type: 'system'
      });
    } else if (this.currentReadings?.batteryVoltage >= 11.5 && this.currentReadings?.batteryVoltage < 12.0) {
      this.systemAlerts.push({
        level: 'Info',
        message: `Battery voltage slightly low (${this.currentReadings?.batteryVoltage}V) - Monitor and consider recharging if the trend continues.`,
        type: 'system'
      });
    }
  
    // Additional checks and suggestions
    // Check for nutrient imbalance (N-P-K ratio)
    const npkRatio = this.currentReadings?.nitrogen / this.currentReadings?.phosphorus / this.currentReadings?.potassium;
    if (npkRatio < 0.5 || npkRatio > 2.0) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Nutrient imbalance detected (N-P-K ratio: ${npkRatio.toFixed(2)}) - Adjust fertilizer application to balance nutrients.`,
        type: 'soil'
      });
    }
  
    // Check for excessive moisture and temperature combination
    if (this.currentReadings?.moisture > 80 && this.currentReadings?.soilTemp > 30) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `High moisture and temperature combination - Risk of fungal diseases. Improve ventilation and reduce watering.`,
        type: 'soil'
      });
    }
  
    // Check for low solar voltage during daylight hours
    if (this.currentReadings?.solarVoltage < 12.0 && this.isDaytime()) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Solar panel underperforming during daylight (${this.currentReadings?.solarVoltage}V) - Inspect for dirt, damage, or shading.`,
        type: 'system'
      });
    }
  }
  
  // Helper method to check if it's daytime (example implementation)
  isDaytime(): boolean {
    const hours = new Date().getHours();
    return hours >= 6 && hours <= 18; // Assuming daytime is between 6 AM and 6 PM
  }

  // Method to update nutrient status based on current readings
  updateNutrientStatus(): void {
    // Initialize nutrient status object if it's empty
    if (!this.nutrientStatus.pH) {
      this.nutrientStatus = {
        phosphorus: { status: '', action: '' },
        potassium: { status: '', action: '' },
        nitrogen: { status: '', action: '' },
        pH: { status: '', action: '' },
        conductivity: { status: '', action: '' }
      };
    }
    
    // Update pH status based on current reading
    const phValue = this.currentReadings?.soilPH;
    if (phValue < 5.0) {
      this.nutrientStatus.pH = { status: 'Very Acidic', action: 'Add lime to raise pH' };
    } else if (phValue < 6.0) {
      this.nutrientStatus.pH = { status: 'Acidic', action: 'Consider adding lime' };
    } else if (phValue < 7.0) {
      this.nutrientStatus.pH = { status: 'Slightly Acidic', action: 'Monitor pH' };
    } else if (phValue === 7.0) {
      this.nutrientStatus.pH = { status: 'Neutral', action: 'Maintain current conditions' };
    } else if (phValue < 8.0) {
      this.nutrientStatus.pH = { status: 'Slightly Alkaline', action: 'Monitor pH' };
    } else {
      this.nutrientStatus.pH = { status: 'Alkaline', action: 'Consider adding sulfur' };
    }

    // Update nitrogen status
    const nitrogenValue = this.currentReadings?.nitrogen;
    if (nitrogenValue < 40) {
      this.nutrientStatus.nitrogen = { status: 'Low', action: 'Add nitrogen-rich amendments' };
    } else if (nitrogenValue < 60) {
      this.nutrientStatus.nitrogen = { status: 'Medium', action: 'Monitor nitrogen levels' };
    } else {
      this.nutrientStatus.nitrogen = { status: 'High', action: 'Reduce nitrogen fertilizers' };
    }

    // Update phosphorus status
    const phosphorusValue = this.currentReadings?.phosphorus;
    if (phosphorusValue < 30) {
      this.nutrientStatus.phosphorus = { status: 'Low', action: 'Add phosphorus-rich amendments' };
    } else if (phosphorusValue < 80) {
      this.nutrientStatus.phosphorus = { status: 'Normal', action: 'Maintain current phosphorus levels' };
    } else {
      this.nutrientStatus.phosphorus = { status: 'High', action: 'Reduce phosphorus fertilizers' };
    }

    // Update potassium status
    const potassiumValue = this.currentReadings?.potassium;
    if (potassiumValue < 40) {
      this.nutrientStatus.potassium = { status: 'Low', action: 'Add potassium-rich amendments' };
    } else if (potassiumValue < 90) {
      this.nutrientStatus.potassium = { status: 'Normal', action: 'Maintain current potassium levels' };
    } else {
      this.nutrientStatus.potassium = { status: 'High', action: 'Reduce potassium fertilizers' };
    }

    // Update conductivity status
    const conductivityValue = this.currentReadings?.conductivity;
    if (conductivityValue < 40) {
      this.nutrientStatus.conductivity = { status: 'Low', action: 'Add potassium-rich amendments' };
    } else  {
      this.nutrientStatus.conductivity = { status: 'Normal', action: 'Maintain current potassium levels' };
    }  
  }

  // Add a method to update all gauge charts when data changes
  updateGaugeCharts(): void {
    // Update soil temperature gauge
    if (this.soilTemperatureGauge) {
      this.soilTemperatureGauge.dataTable = [
        ['Label', 'Value'],
        ['Soil Temp', this.currentReadings?.soilTemp]
      ];
    }

    // Update soil pH gauge
    if (this.soilPHGauge) {
      this.soilPHGauge.dataTable = [
        ['Label', 'Value'],
        ['Soil pH', this.currentReadings?.soilPH]
      ];
    }

    // Update moisture gauge
    if (this.moistureGauge) {
      this.moistureGauge.dataTable = [
        ['Label', 'Value'],
        ['Moisture', this.currentReadings?.moisture]
      ];
    }

    // Update conductivity gauge
    if (this.moistureGauge) {
      this.conductivityGauge.dataTable = [
        ['Label', 'Value'],
        ['Conductivity', this.currentReadings?.conductivity]
      ];
    }

    // Update phosphorus gauge
    if (this.phosphorusGauge) {
      this.phosphorusGauge.dataTable = [
        ['Label', 'Value'],
        ['Phosphorus', this.currentReadings?.phosphorus]
      ];
    }

    // Update potassium gauge
    if (this.potassiumGauge) {
      this.potassiumGauge.dataTable = [
        ['Label', 'Value'],
        ['Potassium', this.currentReadings?.potassium]
      ];
    }

    // Update nitrogen gauge
    if (this.nitrogenGauge) {
      this.nitrogenGauge.dataTable = [
        ['Label', 'Value'],
        ['Nitrogen', this.currentReadings?.nitrogen]
      ];
    }

    // Update solar panel voltage gauge
    if (this.solarPanelVoltageGauge) {
      this.solarPanelVoltageGauge.dataTable = [
        ['Label', 'Value'],
        ['Solar Voltage', this.currentReadings?.solarVoltage]
      ];
    }

    // Update battery voltage gauge
    if (this.batteryVoltageGauge) {
      this.batteryVoltageGauge.dataTable = [
        ['Label', 'Value'],
        ['Battery Voltage', this.currentReadings?.batteryVoltage]
      ];
    }
  }

  // Method to update all UI elements when data changes
  updateDashboard(): void {
    this.updateNutrientStatus();
    this.updateAlerts(); 
    this.updateGaugeCharts();
  }
 

  // Check if there are any critical alerts
  hasCriticalAlerts(): boolean {
    return this.systemAlerts.some(alert => alert.level === 'Critical');
  }

  // Check if there are only warning alerts (no critical ones)
  hasWarningAlertsOnly(): boolean {
    return !this.hasCriticalAlerts() && this.systemAlerts.some(alert => alert.level === 'Warning');
  }

  initializeGaugeCharts(): void {
    // Configuring gauge charts
    const gaugeOptions = {
      width: 250,
      height: 250,
      redFrom: 90,
      redTo: 100,
      yellowFrom: 75,
      yellowTo: 90,
      minorTicks: 5
    };

    this.soilTemperatureGauge = {
      chartType: this.chartType,
      dataTable: [
        ['Label', 'Value'],
        ['Soil Temp', this.currentReadings?.soilTemp]
      ],
      options: {
        ...gaugeOptions,
        min: 0,
        max: 50,
        redFrom: 35,
        redTo: 50,
        yellowFrom: 30,
        yellowTo: 35,
      }
    };

    this.soilPHGauge = {
      chartType: this.chartType,
      dataTable: [
        ['Label', 'Value'],
        ['Soil pH', this.currentReadings?.soilPH]
      ],
      options: {
        ...gaugeOptions,
        min: 0,
        max: 14,
        redFrom: 0,
        redTo: 5,
        yellowFrom: 5,
        yellowTo: 6,
        greenFrom: 6,
        greenTo: 8,
      }
    };

    this.moistureGauge = {
      chartType: this.chartType,
      dataTable: [
        ['Label', 'Value'],
        ['Moisture', this.currentReadings?.moisture]
      ],
      options: {
        ...gaugeOptions,
        min: 0,
        max: 100,
        redFrom: 0,
        redTo: 30,
        yellowFrom: 30,
        yellowTo: 60,
        greenFrom: 60,
        greenTo: 90,
      }
    };

    this.conductivityGauge = {
      chartType: this.chartType,
      dataTable: [
        ['Label', 'Value'],
        ['Conductivity', this.currentReadings?.conductivity]
      ],
      options: {
        ...gaugeOptions,
        min: 0,
        max: 2500,
        redFrom: 0,
        redTo: 30,
        yellowFrom: 30,
        yellowTo: 60,
        greenFrom: 60,
        greenTo: 2000,
      }
    };

    this.phosphorusGauge = {
      chartType: this.chartType,
      dataTable: [
        ['Label', 'Value'],
        ['Phosphorus', this.currentReadings?.phosphorus]
      ],
      options: {
        ...gaugeOptions,
        min: 0,
        max: 150,
        greenFrom: 30,
        greenTo: 80,
        yellowFrom: 80,
        yellowTo: 100,
        redFrom: 100,
        redTo: 150,
      }
    };

    this.potassiumGauge = {
      chartType: this.chartType,
      dataTable: [
        ['Label', 'Value'],
        ['Potassium', this.currentReadings?.potassium]
      ],
      options: {
        ...gaugeOptions,
        min: 0,
        max: 150,
        greenFrom: 40,
        greenTo: 90,
        yellowFrom: 90,
        yellowTo: 110,
        redFrom: 110,
        redTo: 150,
      }
    };

    this.nitrogenGauge = {
      chartType: this.chartType,
      dataTable: [
        ['Label', 'Value'],
        ['Nitrogen', this.currentReadings?.nitrogen]
      ],
      options: {
        ...gaugeOptions,
        min: 0,
        max: 100,
        redFrom: 0,
        redTo: 40,
        yellowFrom: 40,
        yellowTo: 60,
        greenFrom: 60,
        greenTo: 100,
      }
    };

    this.solarPanelVoltageGauge = {
      chartType: this.chartType,
      dataTable: [
        ['Label', 'Value'],
        ['Solar Voltage', this.currentReadings?.solarVoltage]
      ],
      options: {
        ...gaugeOptions,
        min: 0,
        max: 12,
        redFrom: 0,
        redTo: 8,
        yellowFrom: 8,
        yellowTo: 10,
        greenFrom: 10,
        greenTo: 12,
      }
    };

    this.batteryVoltageGauge = {
      chartType: this.chartType,
      dataTable: [
        ['Label', 'Value'],
        ['Battery Voltage', this.currentReadings?.batteryVoltage]
      ],
      options: {
        ...gaugeOptions,
        min: 0,
        max: 12,
        redFrom: 0,
        redTo: 8,
        yellowFrom: 8,
        yellowTo: 10,
        greenFrom: 10,
        greenTo: 12,
      }
    };
  }
 
  initializeLineCharts(): void {
    // Transforming data for Google Charts format
    const temperatureData = [['Day', 'Temperature']];
    const pHData = [['Day', 'pH']];
    const moistureData = [['Day', 'Moisture']];

    if (this.historicalData) {
        this.historicalData.forEach((day, index) => {
            // Keep index as a number
            temperatureData.push([index as any, day.soilTemp]);
            pHData.push([index as any, day.ph]);
            moistureData.push([index as any, day.moisture]);
        });

        const lineChartOptions = {
            height: 200,
            curveType: 'function',
            legend: { position: 'none' },
            hAxis: {
                title: 'Day',
                ticks: this.historicalData.map((day, index) => ({ v: index, f: day.day })) // Map indices to day labels
            },
            vAxis: {
                viewWindow: { min: null, max: null }
            }
        };

        this.temperatureChart = {
            chartType: this.lineChartType,
            dataTable: temperatureData,
            options: {
                ...lineChartOptions,
                title: 'Soil Temperature Trend',
                colors: ['#2563eb'],
                vAxis: {
                    title: 'Temperature (°C)',
                    viewWindow: { min: 0, max: 30 }
                }
            }
        };

        this.pHChart = {
            chartType: this.lineChartType,
            dataTable: pHData,
            options: {
                ...lineChartOptions,
                title: 'Soil pH Trend',
                colors: ['#dc2626'],
                vAxis: {
                    title: 'pH',
                    viewWindow: { min: 0, max: 14 }
                }
            }
        };

        this.moistureChart = {
            chartType: this.lineChartType,
            dataTable: moistureData,
            options: {
                ...lineChartOptions,
                title: 'Moisture Trend',
                colors: ['#d97706'],
                vAxis: {
                    title: 'Moisture %',
                    viewWindow: { min: 0, max: 100 }
                }
            }
        };
    }
}

}
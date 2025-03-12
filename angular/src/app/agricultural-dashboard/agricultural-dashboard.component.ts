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
    if (value < 6)  
      return 'Critical';   
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
      case 'solarVoltage': return this.solarPanelVoltageGauge;
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
    const phValue = this.currentReadings?.soilPH ?? 7.0;
    if (phValue < 5.5) {
      this.systemAlerts.push({
        level: 'Critical',
        message: `Soil pH critically low (${phValue}) - Add agricultural lime or dolomite to raise pH.`,
        type: 'soil'
      });
    } else if (phValue >= 5.5 && phValue < 6.0) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Soil pH slightly low (${phValue}) - Consider adding lime or organic matter to improve pH.`,
        type: 'soil'
      });
    } else if (phValue > 7.5) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Soil pH too high (${phValue}) - Consider adding sulfur or acidifying fertilizers to lower pH.`,
        type: 'soil'
      });
    }

    // Check soil temperature
    const soilTemp = this.currentReadings?.soilTemp ?? 20;
    if (soilTemp > 35) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Soil temperature too high (${soilTemp}°C) - Use mulch or shade to cool the soil and protect plants.`,
        type: 'soil'
      });
    } else if (soilTemp < 10) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Soil temperature too low (${soilTemp}°C) - Consider using row covers or black plastic to warm the soil.`,
        type: 'soil'
      });
    }

    // Check moisture level
    const moisture = this.currentReadings?.moisture ?? 50;
    if (moisture > 90) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Moisture level too high (${moisture}%) - Improve drainage or reduce irrigation to prevent waterlogging.`,
        type: 'soil'
      });
    } else if (moisture < 30) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Moisture level too low (${moisture}%) - Increase irrigation or use mulch to retain soil moisture.`,
        type: 'soil'
      });
    }

    // Check phosphorus level
    const phosphorus = this.currentReadings?.phosphorus ?? 50;
    if (phosphorus < 25) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Phosphorus level low (${phosphorus}ppm) - Apply bone meal or rock phosphate to increase phosphorus.`,
        type: 'soil'
      });
    } else if (phosphorus > 100) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Phosphorus level high (${phosphorus}ppm) - Avoid phosphorus-rich fertilizers to prevent nutrient imbalance.`,
        type: 'soil'
      });
    }

    // Check potassium level
    const potassium = this.currentReadings?.potassium ?? 70;
    if (potassium < 40) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Potassium level low (${potassium}ppm) - Apply potash or wood ash to increase potassium.`,
        type: 'soil'
      });
    } else if (potassium > 300) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Potassium level high (${potassium}ppm) - Avoid potassium-rich fertilizers to prevent nutrient imbalance.`,
        type: 'soil'
      });
    }

    // Check nitrogen level
    const nitrogen = this.currentReadings?.nitrogen ?? 50;
    if (nitrogen < 25) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Nitrogen level low (${nitrogen}ppm) - Apply compost, manure, or nitrogen-rich fertilizers.`,
        type: 'soil'
      });
    } else if (nitrogen > 50) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Nitrogen level high (${nitrogen}ppm) - Avoid nitrogen-rich fertilizers to prevent overgrowth and nutrient runoff.`,
        type: 'soil'
      });
    }

    // Check solar panel voltage
    const solarVoltage = this.currentReadings?.solarVoltage ?? 12;
    if (solarVoltage < 11.0) {
      this.systemAlerts.push({
        level: 'Critical',
        message: `Solar panel voltage critically low (${solarVoltage}V) - Check for obstructions, damage, or disconnections.`,
        type: 'system'
      });
    } else if (solarVoltage >= 11.0 && solarVoltage < 12.0) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Solar panel voltage low (${solarVoltage}V) - Clean the panels or check for shading issues.`,
        type: 'system'
      });
    } else if (solarVoltage >= 12.0 && solarVoltage < 13.0) {
      this.systemAlerts.push({
        level: 'Info',
        message: `Solar panel voltage slightly low (${solarVoltage}V) - Monitor and ensure panels are clean and unobstructed.`,
        type: 'system'
      });
    }

    // Check battery voltage
    const batteryVoltage = this.currentReadings?.batteryVoltage ?? 12;
    if (batteryVoltage < 10.5) {
      this.systemAlerts.push({
        level: 'Critical',
        message: `Battery voltage critically low (${batteryVoltage}V) - Recharge or replace the battery immediately to avoid damage.`,
        type: 'system'
      });
    } else if (batteryVoltage >= 10.5 && batteryVoltage < 11.5) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Battery voltage low (${batteryVoltage}V) - Recharge the battery soon to avoid system shutdown.`,
        type: 'system'
      });
    } else if (batteryVoltage >= 11.5 && batteryVoltage < 12.0) {
      this.systemAlerts.push({
        level: 'Info',
        message: `Battery voltage slightly low (${batteryVoltage}V) - Monitor and consider recharging if the trend continues.`,
        type: 'system'
      });
    }

    // Additional checks and suggestions
    const npkRatio = (this.currentReadings?.nitrogen ?? 50) / (this.currentReadings?.phosphorus ?? 50) / (this.currentReadings?.potassium ?? 70);
    if (npkRatio < 0.5 || npkRatio > 2.0) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Nutrient imbalance detected (N-P-K ratio: ${npkRatio.toFixed(2)}) - Adjust fertilizer application to balance nutrients.`,
        type: 'soil'
      });
    }

    // Check for excessive moisture and temperature combination
    if (moisture > 80 && soilTemp > 30) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `High moisture and temperature combination - Risk of fungal diseases. Improve ventilation and reduce watering.`,
        type: 'soil'
      });
    }

    // Check for low solar voltage during daylight hours
    if (solarVoltage < 12.0 && this.isDaytime()) {
      this.systemAlerts.push({
        level: 'Warning',
        message: `Solar panel underperforming during daylight (${solarVoltage}V) - Inspect for dirt, damage, or shading.`,
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
    if (!this.nutrientStatus || !this.nutrientStatus.pH) {
      this.nutrientStatus = {
        phosphorus: { status: '', action: '' },
        potassium: { status: '', action: '' },
        nitrogen: { status: '', action: '' },
        pH: { status: '', action: '' },
        conductivity: { status: '', action: '' }
      };
    }

    // Update pH status based on current reading
    const phValue = this.currentReadings?.soilPH ?? 7.0;
    if (phValue < 5.5) {
      this.nutrientStatus.pH = { status: 'Very Acidic', action: 'Add lime to raise pH' };
    } else if (phValue < 6.0) {
      this.nutrientStatus.pH = { status: 'Acidic', action: 'Consider adding lime' };
    } else if (phValue < 6.5) {
      this.nutrientStatus.pH = { status: 'Slightly Acidic', action: 'Monitor pH' };
    } else if (phValue <= 7.5) {
      this.nutrientStatus.pH = { status: 'Neutral', action: 'Maintain current conditions' };
    } else if (phValue < 8.5) {
      this.nutrientStatus.pH = { status: 'Slightly Alkaline', action: 'Monitor pH' };
    } else {
      this.nutrientStatus.pH = { status: 'Alkaline', action: 'Consider adding sulfur' };
    }

    // Update nitrogen status
    const nitrogenValue = this.currentReadings?.nitrogen ?? 50;
    if (nitrogenValue < 30) {
      this.nutrientStatus.nitrogen = { status: 'Deficient', action: 'Apply nitrogen-rich fertilizers' };
    } else if (nitrogenValue < 50) {
      this.nutrientStatus.nitrogen = { status: 'Low', action: 'Supplement with nitrogen sources' };
    } else if (nitrogenValue < 80) {
      this.nutrientStatus.nitrogen = { status: 'Optimal', action: 'Maintain nitrogen levels' };
    } else {
      this.nutrientStatus.nitrogen = { status: 'Excessive', action: 'Reduce nitrogen application' };
    }

    // Update phosphorus status
    const phosphorusValue = this.currentReadings?.phosphorus ?? 50;
    if (phosphorusValue < 25) {
      this.nutrientStatus.phosphorus = { status: 'Deficient', action: 'Apply phosphorus-rich amendments' };
    } else if (phosphorusValue < 60) {
      this.nutrientStatus.phosphorus = { status: 'Low', action: 'Increase phosphorus application' };
    } else if (phosphorusValue < 100) {
      this.nutrientStatus.phosphorus = { status: 'Optimal', action: 'Maintain phosphorus levels' };
    } else {
      this.nutrientStatus.phosphorus = { status: 'Excessive', action: 'Reduce phosphorus fertilizers' };
    }

    // Update potassium status
    const potassiumValue = this.currentReadings?.potassium ?? 70;
    if (potassiumValue < 30) {
      this.nutrientStatus.potassium = { status: 'Deficient', action: 'Apply potassium fertilizers' };
    } else if (potassiumValue < 70) {
      this.nutrientStatus.potassium = { status: 'Low', action: 'Increase potassium application' };
    } else if (potassiumValue < 120) {
      this.nutrientStatus.potassium = { status: 'Optimal', action: 'Maintain potassium levels' };
    } else {
      this.nutrientStatus.potassium = { status: 'Excessive', action: 'Reduce potassium fertilizers' };
    }

    // Update conductivity status
    const conductivityValue = this.currentReadings?.conductivity ?? 50;
    if (conductivityValue < 100) {
      this.nutrientStatus.conductivity = { status: 'Low', action: 'Increase soil nutrients' };
    } else if (conductivityValue < 500) {
      this.nutrientStatus.conductivity = { status: 'Moderate', action: 'Maintain soil balance' };
    } else {
      this.nutrientStatus.conductivity = { status: 'High', action: 'Flush soil to prevent salt buildup' };
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
        greenFrom: 15,
        greenTo: 30
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
        max: 8,
        redFrom: 0,
        redTo: 5,
        yellowFrom: 5,
        yellowTo: 6,
        greenFrom: 6,
        greenTo: 7.5 
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
        yellowTo: 50,
        greenFrom: 50,
        greenTo: 100 
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
        redTo: 500,
        yellowFrom: 500,
        yellowTo: 1000,
        greenFrom: 1000,
        greenTo: 2500
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
        redFrom: 0,
        redTo: 30,  
        yellowFrom: 30,
        yellowTo: 50, 
        greenFrom: 50,
        greenTo: 150 
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
        redFrom: 0,
        redTo: 30,  
        yellowFrom: 30,
        yellowTo: 50, 
        greenFrom: 50,
        greenTo: 150 
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
        redTo: 30,    
        yellowFrom: 30,
        yellowTo: 50,  
        greenFrom: 50,
        greenTo: 100   
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
        greenTo: 12
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
        greenTo: 12
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
                    viewWindow: { min: 0, max: 50 }
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
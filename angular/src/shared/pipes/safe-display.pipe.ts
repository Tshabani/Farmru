import { Injector, Pipe, PipeTransform } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';

@Pipe({
  name: 'safeDisplay'
})
export class SafeDisplayPipe  extends AppComponentBase implements PipeTransform {
    
    constructor(injector: Injector) {
        super(injector);
    }  

    transform(value: any, property: string): string {
    return value && value[property] ? value[property] : '';
  }
}

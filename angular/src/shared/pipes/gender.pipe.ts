import { Injector, Pipe, PipeTransform } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { getGenderOptions } from '@shared/helpers/userHealper';
import { RefListGender } from '@shared/service-proxies/service-proxies';

@Pipe({
    name: 'gender'
})
export class GenderPipe extends AppComponentBase implements PipeTransform {

    constructor(injector: Injector) {
        super(injector);
    }

    transform(value: RefListGender): string {
        const option = getGenderOptions().find(option => option.value === value);
        return option ? option.label : 'Unknown';
      }
}

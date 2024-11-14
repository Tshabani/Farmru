import {
    RefListPersonTitle,
    RefListGender
  } from '../service-proxies/service-proxies';

export interface TitleOption {
    value: RefListPersonTitle;
    label: string;
}
export function getPersonTitles() {
    return [
      { value: RefListPersonTitle._0, label: 'Unknown' },
      { value: RefListPersonTitle._1, label: 'Mr' },
      { value: RefListPersonTitle._2, label: 'Mrs' },
      { value: RefListPersonTitle._3, label: 'Miss' },
      { value: RefListPersonTitle._4, label: 'Ms' }
    ];
  }

export interface GenderOption {
    value: RefListGender;
    label: string;
}
  
export function getGenderOptions(): GenderOption[] {
  return [
    { value: RefListGender._0, label: 'Unknown' },
    { value: RefListGender._1, label: 'Male' },
    { value: RefListGender._2, label: 'Female' }
  ];
}


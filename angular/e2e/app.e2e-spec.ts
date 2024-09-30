import { IotMonitoringTemplatePage } from './app.po';

describe('IotMonitoring App', function() {
  let page: IotMonitoringTemplatePage;

  beforeEach(() => {
    page = new IotMonitoringTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});

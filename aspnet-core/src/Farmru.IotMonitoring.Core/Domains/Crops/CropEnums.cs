namespace Farmru.IotMonitoring.Domains.Crops
{
    public enum CropSeasonStatus
    {
        Planned = 0,
        Growing = 1,
        Harvested = 2,
        Closed = 3
    }

    public enum GrowthStage
    {
        Planted = 0,
        Germination = 1,
        Vegetative = 2,
        Flowering = 3,
        Fruiting = 4,
        Maturity = 5,
        Harvested = 6
    }

    public enum GrowthStageSource
    {
        Manual = 0,
        Satellite = 1
    }
}

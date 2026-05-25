using Farmru.IotMonitoring.Geo;

namespace Farmru.IotMonitoring.Domains.Incidents
{
    public partial class Incident
    {
        public virtual decimal? Latitude { get; private set; }
        public virtual decimal? Longitude { get; private set; }

        public virtual void SetLocation(decimal? latitude, decimal? longitude)
        {
            var coords = GeoCoordinateHelper.Normalize(latitude, longitude);
            if (!coords.HasValue)
            {
                Latitude = null;
                Longitude = null;
                return;
            }

            Latitude = coords.Value.Latitude;
            Longitude = coords.Value.Longitude;
        }
    }
}

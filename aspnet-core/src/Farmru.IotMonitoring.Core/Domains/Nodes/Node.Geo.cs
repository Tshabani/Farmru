using Farmru.IotMonitoring.Geo;
using System;

namespace Farmru.IotMonitoring.Domains.Nodes
{
    public partial class Node
    {
        public virtual decimal? LastKnownLatitude { get; private set; }
        public virtual decimal? LastKnownLongitude { get; private set; }

        public virtual void UpdateKnownLocation(long? latitude, long? longitude)
        {
            var coords = GeoCoordinateHelper.FromDeviceReading(latitude, longitude);
            if (!coords.HasValue)
            {
                return;
            }

            LastKnownLatitude = coords.Value.Latitude;
            LastKnownLongitude = coords.Value.Longitude;
        }

        public virtual void UpdateKnownLocation(decimal? latitude, decimal? longitude)
        {
            var coords = GeoCoordinateHelper.Normalize(latitude, longitude);
            if (!coords.HasValue)
            {
                return;
            }

            LastKnownLatitude = coords.Value.Latitude;
            LastKnownLongitude = coords.Value.Longitude;
        }

        public virtual (decimal Latitude, decimal Longitude)? ResolveMapCoordinates()
        {
            if (LastKnownLatitude.HasValue && LastKnownLongitude.HasValue)
            {
                return (LastKnownLatitude.Value, LastKnownLongitude.Value);
            }

            if (Facility?.Latitude != null && Facility?.Longitude != null)
            {
                return (Facility.Latitude.Value, Facility.Longitude.Value);
            }

            return null;
        }
    }
}

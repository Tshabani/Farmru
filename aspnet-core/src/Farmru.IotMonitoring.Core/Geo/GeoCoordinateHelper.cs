using Farmru.IotMonitoring.Domains;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace Farmru.IotMonitoring.Geo
{
    public static class GeoCoordinateHelper
    {
        public const decimal MinLatitude = -90m;
        public const decimal MaxLatitude = 90m;
        public const decimal MinLongitude = -180m;
        public const decimal MaxLongitude = 180m;

        public static (decimal Latitude, decimal Longitude)? Normalize(decimal? latitude, decimal? longitude)
        {
            if (!latitude.HasValue || !longitude.HasValue)
            {
                return null;
            }

            var lat = latitude.Value;
            var lon = longitude.Value;

            if (!IsValid(lat, lon))
            {
                throw new DomainRuleException("Coordinates must be within valid latitude/longitude ranges.");
            }

            return (Math.Round(lat, 8), Math.Round(lon, 8));
        }

        public static (decimal Latitude, decimal Longitude)? FromDeviceReading(long? latitude, long? longitude)
        {
            if (!latitude.HasValue || !longitude.HasValue)
            {
                return null;
            }

            decimal lat = latitude.Value;
            decimal lon = longitude.Value;

            if (Math.Abs(lat) > 180 || Math.Abs(lon) > 180)
            {
                lat = latitude.Value / 10_000_000m;
                lon = longitude.Value / 10_000_000m;
            }

            return Normalize(lat, lon);
        }

        public static bool IsValid(decimal latitude, decimal longitude) =>
            latitude >= MinLatitude && latitude <= MaxLatitude &&
            longitude >= MinLongitude && longitude <= MaxLongitude;

        public static double DistanceMeters(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const double earthRadius = 6371000;
            var dLat = ToRadians((double)(lat2 - lat1));
            var dLon = ToRadians((double)(lon2 - lon1));
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadius * c;
        }

        public static bool IsInsideRadius(decimal pointLat, decimal pointLon, decimal centerLat, decimal centerLon, double radiusMeters) =>
            DistanceMeters(pointLat, pointLon, centerLat, centerLon) <= radiusMeters;

        public static bool IsInsidePolygon(decimal pointLat, decimal pointLon, string polygonJson)
        {
            var points = ParsePolygon(polygonJson);
            if (points.Count < 3)
            {
                return false;
            }

            var inside = false;
            var j = points.Count - 1;
            for (var i = 0; i < points.Count; i++)
            {
                var pi = points[i];
                var pj = points[j];
                if ((pi.Lat > (double)pointLat) != (pj.Lat > (double)pointLat) &&
                    (double)pointLon < (pj.Lng - pi.Lng) * ((double)pointLat - pi.Lat) / (pj.Lat - pi.Lat) + pi.Lng)
                {
                    inside = !inside;
                }

                j = i;
            }

            return inside;
        }

        public static List<(double Lat, double Lng)> ParsePolygon(string polygonJson)
        {
            if (string.IsNullOrWhiteSpace(polygonJson))
            {
                return new List<(double Lat, double Lng)>();
            }

            try
            {
                var doc = JsonDocument.Parse(polygonJson);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    return doc.RootElement.EnumerateArray()
                        .Select(ReadPoint)
                        .Where(p => p.HasValue)
                        .Select(p => p.Value)
                        .ToList();
                }
            }
            catch (JsonException)
            {
                return new List<(double Lat, double Lng)>();
            }

            return new List<(double Lat, double Lng)>();
        }

        private static (double Lat, double Lng)? ReadPoint(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                if (element.TryGetProperty("lat", out var latEl) && element.TryGetProperty("lng", out var lngEl))
                {
                    return (latEl.GetDouble(), lngEl.GetDouble());
                }

                if (element.TryGetProperty("latitude", out var lat2) && element.TryGetProperty("longitude", out var lng2))
                {
                    return (lat2.GetDouble(), lng2.GetDouble());
                }
            }

            return null;
        }

        private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
    }
}

using System;
using System.Xml.Serialization;

namespace Core
{
    public class GPSLocation
    {
        [XmlAttribute]
        public double Longitude { get; set; }
        [XmlAttribute]
        public double Latitude { get; set; }

        public GPSLocation(double latitude, double longitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public GPSLocation()
        {
            Longitude = 0;
            Latitude = 0;
        }

        public bool Equals(GPSLocation other)
        {
            return Longitude.Equals(other.Longitude) && Latitude.Equals(other.Latitude);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is GPSLocation && Equals((GPSLocation)obj);
        }
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                var i = Longitude.GetHashCode() * 397;
                return i ^ Latitude.GetHashCode();
            }
        }

        public static bool operator ==(GPSLocation left, GPSLocation right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GPSLocation left, GPSLocation right)
        {
            return !Equals(left, right);
        }

        public static double operator -(GPSLocation location1, GPSLocation location2)
        {
            return GetDistanceInMeter(location1, location2);
        }

        //From https://www.movable-type.co.uk/scripts/latlong.html
        public static double GetDistanceInMeter(GPSLocation location1, GPSLocation location2)
        {
            const double radius = 6371000;
            var latRad1 = ToRadians(location1.Latitude);
            var latRad2 = ToRadians(location2.Latitude);
            var deltaLat = ToRadians(location2.Latitude - location1.Latitude);
            var deltaLong = ToRadians(location2.Longitude - location1.Longitude);

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2);
            a += Math.Cos(latRad1) * Math.Cos(latRad2) * Math.Sin(deltaLong / 2) * Math.Sin(deltaLong / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = radius * c;
            return d;
        }

        private static double ToRadians(double x)
        {
            return x / 180 * Math.PI;
        }
    }
}
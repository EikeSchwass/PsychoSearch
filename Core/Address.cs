using System;

namespace Core
{
    public class Address
    {
        public string Road { get; set; } = "";
        public string HouseNumber { get; set; } = "";
        public string ZipCode { get; set; } = "";
        public string City { get; set; } = "";

        public override string ToString()
        {
            return $"{Road} {HouseNumber} {Environment.NewLine}{ZipCode} {City}";
        }
    }
}
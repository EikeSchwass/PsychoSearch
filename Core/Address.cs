using System;

namespace Core
{
    public class Address
    {
        public string Street { get; set; } = "";
        public string City { get; set; } = "";

        public override string ToString()
        {
            return $"{Street} {Environment.NewLine}{City}";
        }
    }
}
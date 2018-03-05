using System;

namespace PsychoAssist.Core
{
    public class Address
    {
        public string Street { get; set; } = "";
        public string City { get; set; } = "";

        public string FullAddress => ToString();

        public override string ToString()
        {
            return $"{Street}, {Environment.NewLine}{City}";
        }
    }
}
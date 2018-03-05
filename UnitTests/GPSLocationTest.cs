using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class GPSLocationTest
    {
        [TestMethod]
        public void DistanceTest()
        {
            var location1 = new GPSLocation(40.7, -15.5);
            var location2 = new GPSLocation(40.9, -15.0);
            var location3 = new GPSLocation(40.7, -16);
            var location4 = new GPSLocation(-10.9, -10.0);
            var d1 = GPSLocation.GetDistanceInMeter(location1, location2);
            var d2 = GPSLocation.GetDistanceInMeter(location2, location3);
            var d3 = GPSLocation.GetDistanceInMeter(location3, location4);
            var d4 = GPSLocation.GetDistanceInMeter(location4, location1);
            Assert.AreEqual(47600, d1, d1 * 0.0001);
            Assert.AreEqual(87060, d2, d2 * 0.0001);
            Assert.AreEqual(5771000, d3, d3 * 0.0001);
            Assert.AreEqual(5765000, d4, d4 * 0.0001);
            Assert.AreEqual(location1 - location2, location2 - location1, 0.001);
            Assert.AreEqual(location2 - location3, location3 - location2, 0.001);
        }
    }
}

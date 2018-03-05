using System;
using System.Xml.Serialization;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class SerializationTest
    {
        [TestMethod]
        public void Test()
        {
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new XmlSerializer(typeof(Therapist[]));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
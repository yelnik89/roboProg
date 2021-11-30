using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using roboProg;

namespace RoboProgTests
{
    [TestClass]
    public class ConvertDataToSaveTest
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void ConvertDataForPolitaser()
        {
            ConvertDataToSave convertData = new ConvertDataToSave("P", "poligon");
            string politaserStingFromPoligon = "";
            convertData.getDictionary(politaserStingFromPoligon);
        }
    }
}

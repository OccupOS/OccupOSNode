namespace OccupOS.Common.Tests {
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OccupOS.CommonLibrary.NodeControllers;
    using OccupOS.CommonLibrary.HardwareControllers;
    using OccupOS.CommonLibrary.Sensors;

    public class TestHardwareController : HardwareController
    {
    }

    public class TestSensor : Sensor
    {
        public TestSensor(int id)
            : base(id)
        {
        }

        public override SensorData GetData() {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public class NodeControllerTests {

        [TestMethod]
        public void GetSensorByIndex()
        {
            TestHardwareController hwController = new TestHardwareController();

            TestSensor sensor = new TestSensor(1);
            hwController.AddSensor(sensor);

            Assert.AreEqual(sensor, hwController.GetSensor(0));
        }
    }
}
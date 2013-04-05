﻿namespace OccupOS.Common.Tests {
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OccupOS.CommonLibrary.NodeControllers;
    using OccupOS.CommonLibrary.Sensors;

    public class TestNodeController : NodeController
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
    public class ProgramTests {

        [TestMethod]
        public void GetSensorByIndex()
        {
            TestNodeController nodeController = new TestNodeController();

            TestSensor sensor = new TestSensor(1);
            nodeController.AddSensor(sensor);

            Assert.AreEqual(sensor, nodeController.GetSensor(0));
        }
    }
}
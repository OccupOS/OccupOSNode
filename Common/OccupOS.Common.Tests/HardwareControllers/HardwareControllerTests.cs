// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareControllerTests.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOS.Common.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OccupOS.CommonLibrary.HardwareControllers;
    using OccupOS.CommonLibrary.Sensors;

    [TestClass]
    public class HardwareControllerTests
    {
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void AddingNullSensorThrowsException()
        {
            var hardwareController = new HardwareController();
            hardwareController.AddSensor(null);

            Assert.Fail("Exception expected.");
        }

        [TestMethod]
        public void GetAllSensorsWithNoSensorsReturnsEmpty()
        {
            var hardwareController = new HardwareController();
            Assert.AreEqual(0, hardwareController.GetAllSensors().Count);
        }

        [TestMethod]
        public void GetSensorByIndex()
        {
            var hardwareController = new TestHardwareController();

            var sensor = new TestSensor(1);
            hardwareController.AddSensor(sensor);

            Assert.AreEqual(sensor, hardwareController.GetSensor(0));
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetSensorByIndexThrowsException()
        {
            var hardwareController = new TestHardwareController();

            var sensor = new TestSensor(1);
            hardwareController.AddSensor(sensor);

            hardwareController.GetSensor(4);
        }

        [TestMethod]
        public void GetSensorCount()
        {
            var random = new Random();
            var hardwareController = new TestHardwareController();

            int sensorCount = random.Next(0, 20);
            for (int i = 0; i < sensorCount; i++)
            {
                var sensor = new TestSensor(i);
                hardwareController.AddSensor(sensor);
            }

            Assert.AreEqual(sensorCount, hardwareController.GetSensorCount());
        }

        [TestMethod]
        public void GetSensorCountByType()
        {
            var random = new Random();
            var hardwareController = new TestHardwareController();

            int sensorCount = random.Next(0, 20);
            for (int i = 0; i < sensorCount; i++)
            {
                var sensor = new TestSensor(i);
                hardwareController.AddSensor(sensor);
            }

            int sensorBCount = random.Next(0, 20);
            for (int i = 0; i < sensorBCount; i++)
            {
                var sensor = new TestSensorB(i);
                hardwareController.AddSensor(sensor);
            }

            Assert.AreEqual(sensorBCount, hardwareController.GetSensorCount(typeof(TestSensorB)));
        }

        [TestMethod]
        public void GetSensorDataBufferCount()
        {
            var hardwareController = new HardwareController();
            var random = new Random();

            int dataCount = random.Next(1, 5);
            for (int i = 0; i < dataCount; i++)
            {
                SensorData[] sensorDatas = new SensorData[random.Next(1, 5)];
                for (int j = 0; j < sensorDatas.Length; j++)
                {
                    var sensorData = new SensorData
                                         {
                                             Humidity = random.Next(0, 100) / 100, 
                                             Pressure = random.Next(0, 100) / 100
                                         };

                    sensorDatas[j] = sensorData;
                }

                hardwareController.AddSensorReadings(sensorDatas);
            }

            Assert.AreEqual(dataCount, hardwareController.GetSensorDataBufferCount());
        }

        [TestMethod]
        public void GetSensorReadings()
        {
            var hardwareController = new HardwareController();
            var random = new Random();

            SensorData[] sensorDatas = new SensorData[random.Next(1, 5)];
            for (int i = 0; i < sensorDatas.Length; i++)
            {
                var sensorData = new SensorData
                                     {
                                         Humidity = random.Next(0, 100) / 100, 
                                         Pressure = random.Next(0, 100) / 100
                                     };

                sensorDatas[i] = sensorData;
            }

            hardwareController.AddSensorReadings(sensorDatas);

            Assert.AreEqual(sensorDatas, hardwareController.GetSensorReadings(0));
        }

        [TestMethod]
        public void GetSensorReadingsByIndex()
        {
            var hardwareController = new HardwareController();
            var random = new Random();

            SensorData[] sensorDatas = new SensorData[random.Next(1, 5)];
            for (int i = 0; i < sensorDatas.Length; i++)
            {
                var sensorData = new SensorData
                                     {
                                         Humidity = random.Next(0, 100) / 100, 
                                         Pressure = random.Next(0, 100) / 100
                                     };

                sensorDatas[i] = sensorData;
            }

            hardwareController.AddSensorReadings(sensorDatas);
            hardwareController.RemoveSensorReadings(0);

            Assert.AreEqual(0, hardwareController.GetSensorDataBufferCount());
        }

        [TestMethod]
        public void PollSensorReadingsRemovesSensorData()
        {
            var hardwareController = new HardwareController();
            var random = new Random();

            SensorData[] sensorDatas = new SensorData[random.Next(1, 5)];
            for (int i = 0; i < sensorDatas.Length; i++)
            {
                var sensorData = new SensorData
                                     {
                                         Humidity = random.Next(0, 100) / 100, 
                                         Pressure = random.Next(0, 100) / 100
                                     };

                sensorDatas[i] = sensorData;
            }

            hardwareController.AddSensorReadings(sensorDatas);
            hardwareController.PollSensorReadings(0);

            Assert.AreEqual(0, hardwareController.GetSensorDataBufferCount());
        }

        [TestMethod]
        public void RemoveSensor()
        {
            var hardwareController = new TestHardwareController();

            var sensor = new TestSensor(1);
            hardwareController.AddSensor(sensor);

            hardwareController.RemoveSensor(0);

            Assert.AreEqual(0, hardwareController.GetSensorCount());
        }

        [TestMethod]
        public void RemoveSensorByID()
        {
            var hardwareController = new TestHardwareController();

            var sensor = new TestSensor(1);
            hardwareController.AddSensor(sensor);

            hardwareController.RemoveSensorByID(1);

            Assert.AreEqual(0, hardwareController.GetSensorCount());
        }

        private class TestHardwareController : HardwareController
        {
        }

        private class TestSensor : Sensor
        {
            public TestSensor(int id)
                : base(id)
            {
            }

            public override SensorData GetData()
            {
                throw new NotImplementedException();
            }
        }

        private class TestSensorB : Sensor
        {
            public TestSensorB(int id)
                : base(id)
            {
            }

            public override SensorData GetData()
            {
                throw new NotImplementedException();
            }
        }
    }
}
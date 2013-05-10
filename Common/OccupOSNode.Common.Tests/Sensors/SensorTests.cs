// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SensorTests.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace OccupOS.Common.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OccupOS.CommonLibrary.Sensors;

    [TestClass]
    public class SensorTests
    {
        [TestMethod]
        public void GetSensorID()
        {
            const int ID = 1;
            var testSensor = new TestSensor(ID);

            Assert.AreEqual(ID, testSensor.ID);
        }

        private class TestSensor : Sensor
        {
            public TestSensor(int id)
                : base(id)
            {
            }

            public override SensorData GetData()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgramTests.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOSNode
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OccupOS.CommonLibrary.Sensors;

    [TestClass]
    public class ProgramTests
    {
        // TODO: Add program tests.
    }

    [TestClass]
    public class KinectRunnerTests
    {
        #region Public Methods and Operators

        [TestMethod]
        public void PackDemoDataForm()
        {
            KinectRunner kinectRunner = new KinectRunner();
            Random random = new Random();

            SensorData sensorData = new SensorData();
            sensorData.EntityCount = random.Next(0, 6);

            Position[] positions = new Position[2];

            for (int i = 0; i < positions.Length; i++)
            {
                Position position = new Position
                                        {
                                            X = random.Next(0, 100), 
                                            Y = random.Next(0, 100), 
                                            Depth = random.Next(0, 100) / 100
                                        };

                positions[i] = position;
            }

            sensorData.EntityPositions = positions;

            string packedData = kinectRunner.DemoDataForm(sensorData);
            Assert.AreEqual(
                sensorData.EntityCount + "," + sensorData.EntityPositions[0].X + "," + sensorData.EntityPositions[0].Y
                                       + "," + sensorData.EntityPositions[0].Depth + ","
                                       + sensorData.EntityPositions[1].X + "," + sensorData.EntityPositions[1].Y + ","
                                       + sensorData.EntityPositions[1].Depth, 
                packedData);
        }

        [TestMethod]
        public void PackDemoDataFormWithIrrelevantData()
        {
            KinectRunner kinectRunner = new KinectRunner();
            Random random = new Random();

            SensorData sensorData = new SensorData();
            sensorData.AnalogLight = random.Next(0, 100);
            sensorData.Humidity = random.Next(0, 1000) / 100;
            sensorData.PollTime = DateTime.Now;
            sensorData.Pressure = random.Next(0, 1000) / 100;
            sensorData.ReadTime = DateTime.Now;
            sensorData.Temperature = random.Next(0, 1000) / 100;

            string packedData = kinectRunner.DemoDataForm(sensorData);
            Assert.AreEqual("0", packedData);
        }

        #endregion
    }
}
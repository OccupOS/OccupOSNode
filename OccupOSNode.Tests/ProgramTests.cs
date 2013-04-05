namespace OccupOSNode
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OccupOS.CommonLibrary.NodeControllers;
    using OccupOS.CommonLibrary.Sensors;

    [TestClass]
    public class ProgramTests
    {
        // TODO: Add program tests.
    }

    [TestClass]
    public class KinectRunnerTests
    {
        [TestMethod]
        public void PackDemoDataForm()
        {
            KinectRunner kinectRunner = new KinectRunner();
            Random random = new Random();
            
            SensorData sensorData = new SensorData();
            sensorData.EntityCount = 1;
            
            Position[] positions = new Position[1];

            for (int i = 0; i < positions.Length; i++)
            {
                Position position = new Position();
                position.X = random.Next(0, 100);
                position.Y = random.Next(0, 100);
                position.Depth = random.Next(0, 100) / 100;

                positions[i] = position;
            }

            sensorData.EntityPositions = positions;

            string packedData = kinectRunner.DemoDataForm(sensorData);
            Assert.AreEqual(sensorData.EntityCount + "," +
                            sensorData.EntityPositions[0].X + "," +
                            sensorData.EntityPositions[0].Y + "," +
                            sensorData.EntityPositions[0].Depth,
                            packedData);
        }

        [TestMethod]
        public void PackDemoDataFormWithExtraData()
        {
            // TODO: We should fill some random sensor extra sensor data as well.
        }
    }
}
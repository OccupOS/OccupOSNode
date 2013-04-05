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
            sensorData.EntityCount = random.Next(0, 6);
            
            Position[] positions = new Position[2];

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
                            sensorData.EntityPositions[0].Depth + "," +
                            sensorData.EntityPositions[1].X + "," +
                            sensorData.EntityPositions[1].Y + "," +
                            sensorData.EntityPositions[1].Depth,
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
    }
}
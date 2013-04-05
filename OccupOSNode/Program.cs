// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="OccupOS">
//   This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License along with this program. If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace OccupOSNode {

    using System;
    using System.Threading;
    using OccupOSNode.Sensors.Kinect;
    using System.Reflection;
    using OccupOS.CommonLibrary.Sensors;
    using OccupOS.CommonLibrary.NodeControllers;
    using OccupOSNode.NetworkControllers;
    using OccupOS.CommonLibrary;
    using OccupOSCloud; //this and TestServer reference are temporary

    internal class Program {
        private static void Main(string[] args) {

            FullEthernetController ncontroller = new FullEthernetController("UrsaMinor", 1333);
            while (!ncontroller.Connect("", "")) { }

            SensorData testdata = new SensorData();
            testdata.Humidity = 10;
            testdata.Pressure = 10;
            testdata.Temperature = 10;

            while (true) {
                Console.WriteLine("Attempting send...");
                ncontroller.SendData(PacketFactory.CreatePacket(testdata));
                Console.WriteLine("Sent!");
                System.Threading.Thread.Sleep(1000);
            }

            /*var kinectrunner = new KinectRunner();
            var kthread = new Thread(kinectrunner.DelayedPoll);
            kthread.Start();*/
        }
    }

    public class KinectRunner {
        public void DelayedPoll() {
            FullNodeController controller = new FullNodeController();
            controller.StartListening();

            while (true) {
                if (controller.GetSensorCount() > 0) {
                    try {
                        SensorData data = ((NodeKinectSensor)controller.GetSensor(0)).GetData();
                        string send_data = DemoDataForm(data);
                        Console.WriteLine("Sending: " + send_data);
                        var helper = new SQLServerHelper("tcp:dndo40zalb.database.windows.net,1433", "comp2014@dndo40zalb", "20041908kjH", "TestSQLDB");
                        helper.InsertSensorData(3, 1, send_data, DateTime.Now, 1);
                    } catch (Exception e) { }
                }
                Thread.Sleep(5000);
            }
        }

        public string DemoDataForm(SensorData data_in)
        {
            string sensordata = "";
            sensordata = sensordata + data_in.EntityCount;
            if (data_in.EntityPositions != null)
            {
                int len = data_in.EntityPositions.Length;
                if (len > 0)
                {
                    sensordata = sensordata + ",";
                    int k = 0;
                    foreach (Position pos in data_in.EntityPositions)
                    {
                        sensordata = sensordata + pos.X + "," + pos.Y + "," + pos.Depth;
                        k++;
                        if (k < len) sensordata = sensordata + ",";
                    }
                }
            }
            return sensordata;
        }
    }
}
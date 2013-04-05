// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOSNode
{
    using System;
    using System.Threading;

    using OccupOS.CommonLibrary;
    using OccupOS.CommonLibrary.Sensors;

    using OccupOSCloud;

    using OccupOSNode.NetworkControllers;

    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            var ncontroller = new FullEthernetController("UrsaMinor", 1333);
            while (!ncontroller.Connect(string.Empty, string.Empty))
            {
            }

            var testdata = new SensorData { Humidity = 10, Pressure = 10, Temperature = 10 };

            while (true)
            {
                Console.WriteLine("Attempting send...");
                ncontroller.SendData(PacketFactory.CreatePacket(testdata));
                Console.WriteLine("Sent!");
                Thread.Sleep(1000);
            }

            /*var kinectrunner = new KinectRunner();
            var kthread = new Thread(kinectrunner.DelayedPoll);
            kthread.Start();*/
        }

        #endregion
    }

    public class KinectRunner
    {
        #region Public Methods and Operators

        public void DelayedPoll()
        {
            var controller = new FullNodeController();
            controller.StartListening();

            while (true)
            {
                if (controller.GetSensorCount() > 0)
                {
                    try
                    {
                        SensorData data = controller.GetSensor(0).GetData();
                        string sendData = this.DemoDataForm(data);
                        Console.WriteLine("Sending: " + sendData);
                        var helper = new SQLServerHelper(
                            "tcp:dndo40zalb.database.windows.net,1433", 
                            "comp2014@dndo40zalb", 
                            "20041908kjH", 
                            "TestSQLDB");
                        helper.InsertSensorData(3, 1, sendData, DateTime.Now, 1);
                    }
                    catch (Exception e)
                    {
                        Console.Write(e.Message);
                    }
                }

                Thread.Sleep(5000);
            }
        }

        public string DemoDataForm(SensorData dataIn)
        {
            string sensordata = string.Empty;
            sensordata = sensordata + dataIn.EntityCount;
            if (dataIn.EntityPositions != null)
            {
                int length = dataIn.EntityPositions.Length;
                if (length > 0)
                {
                    sensordata = sensordata + ",";
                    int k = 0;
                    foreach (Position pos in dataIn.EntityPositions)
                    {
                        sensordata = sensordata + pos.X + "," + pos.Y + "," + pos.Depth;
                        k++;
                        if (k < length)
                        {
                            sensordata = sensordata + ",";
                        }
                    }
                }
            }

            return sensordata;
        }

        #endregion
    }
}
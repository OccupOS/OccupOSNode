﻿// --------------------------------------------------------------------------------------------------------------------
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
namespace OccupOSNode
{
    using System;
    using System.Threading;
    using OccupOSNode.Sensors.Kinect;
    using System.Reflection;
    using OccupOS.CommonLibrary.Sensors;
    using OccupOS.CommonLibrary.NodeControllers;

    internal class Program
    {
        private static void Main(string[] args)
        {
            FullNodeController controller = new FullNodeController();
            while (true) {
                controller.StopListening();
                for (int k = 0; k < 60000; k++) { Console.Write("Off: " + k + ": "); Console.WriteLine(controller.GetSensorCount()); }
                controller.StartListening();
                for (int k = 0; k < 60000; k++) { Console.Write("On: " + k + ": "); Console.WriteLine(controller.GetSensorCount()); }
            }

            //var kinectrunner = new KinectRunner();
            //var kthread = new Thread(kinectrunner.DelayedPoll);
            //kthread.Start();
        }
    }

    public class KinectRunner
    {
        public void DelayedPoll()
        {
            var testsensor = new NodeKinectSensor(100);

            while (true)
            {
                Thread.Sleep(5000);
                int count = testsensor.GetEntityCount();
                Console.WriteLine("Sending: " + count);
                /*var test = new SQLServerHelper(
                    "tcp:dndo40zalb.database.windows.net,1433", "comp2014@dndo40zalb", "20041908kjH", "TestSQLDB");
                test.insertSensorData(
                    1, 1, count.ToString(), DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);

                test.insertSensorData(1, 1, "7", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now); 
                fix timeout expired, keep sending even after error*/
            }
        }
    }
}
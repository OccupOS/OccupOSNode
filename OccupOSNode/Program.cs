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
namespace OccupOSNode
{
    using System;
    using System.Threading;

    using OccupOSCloud;

    using OccupOSNode.Sensors.Kinect;

    /// <summary>
    ///     The program.
    /// </summary>
    internal class Program
    {
        #region Methods

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Main(string[] args)
        {
            var kinectrunner = new KinectRunner();
            var kthread = new Thread(kinectrunner.DelayedPoll);
            kthread.Start();
        }

        #endregion
    }

    /// <summary>
    ///     The kinect runner.
    /// </summary>
    public class KinectRunner
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The delayed poll.
        /// </summary>
        public void DelayedPoll()
        {
            var testsensor = new NodeKinectSensor("testsensor");

            while (true)
            {
                Thread.Sleep(5000);
                int count = testsensor.GetEntityCount();
                Console.WriteLine("Sending: " + count);
                var test = new SQLServerHelper(
                    "tcp:dndo40zalb.database.windows.net,1433", "comp2014@dndo40zalb", "20041908kjH", "TestSQLDB");
                test.insertSensorData(
                    1, 1, count.ToString(), DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);

                // test.insertSensorData(1, 1, "7", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now); 
                // fix timeout expired, keep sending even after error
            }
        }

        #endregion
    }
}
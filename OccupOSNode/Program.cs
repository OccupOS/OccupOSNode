using System;
using System.Threading;
using OccupOSCloud;
using OccupOSNode.Sensors.Kinect;

namespace OccupOSNode {
    internal class Program {
        private static void Main(string[] args) {
            var kinectrunner = new KinectRunner();
            var kthread = new Thread(kinectrunner.DelayedPoll);
            kthread.Start();
        }
    }

    public class KinectRunner {
        public void DelayedPoll() {
            var testsensor = new NodeKinectSensor("testsensor");

            while (true) {
                Thread.Sleep(5000);
                int count = testsensor.GetEntityCount();
                Console.WriteLine("Sending: " + count);
                var test = new SQLServerHelper("tcp:dndo40zalb.database.windows.net,1433",
                                               "comp2014@dndo40zalb",
                                               "20041908kjH",
                                               "TestSQLDB");
                test.insertSensorData(1,
                                      1,
                                      (count.ToString()),
                                      DateTime.Now,
                                      DateTime.Now,
                                      DateTime.Now,
                                      DateTime.Now,
                                      DateTime.Now);
                //test.insertSensorData(1, 1, "7", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now); 
                //fix timeout expired, keep sending even after error
            }
        }
    }
}
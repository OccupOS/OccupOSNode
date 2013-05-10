// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using GT = Gadgeteer;

namespace GadgeteerDemo
{
    using System;
    using System.Threading;

    using Microsoft.SPOT;

    using OccupOS.CommonLibrary;
    using OccupOS.CommonLibrary.Sensors;

    public partial class Program
    {
        private readonly GT.Timer timer = new GT.Timer(2000);

        private GadgeteerWiFiNetworkController networkController;

        private GadgeteerSensor sensor;

        // This method is run when the mainboard is powered up or reset. 
        private void ProgramStarted()
        {
            Microsoft.SPOT.Hardware.Utility.SetLocalTime(DateTime.Now);
            this.sensor = new GadgeteerSensor(10); // id 10
            this.networkController = new GadgeteerWiFiNetworkController(this.wifi_RS21);
            this.networkController.ConnectToWiFi("2WIRE487", "0046056798");
            this.networkController.ConnectToSocket("192.168.1.65", 1333);

            this.timer.Tick += this.timer_Tick;
            this.timer.Start();

            Debug.Print("Finished setup");
        }

        private void timer_Tick(GT.Timer timer)
        {
            this.sensor.SetAnalogLightValue((int)this.lightSensor.ReadLightSensorPercentage());
            Debug.Print("Sending data: AnalogLight - " + this.sensor.GetAnalogLightValue());

            SensorData[] databundle = new[] { this.sensor.GetData() };
            string packet = PacketFactory.SerializeJSON(2, databundle); // node id 2
            this.networkController.SendData(packet);

            Thread.Sleep(10000);
        }
    }

    public class GadgeteerSensor : Sensor, ILightSensor
    {
        private float lightvalue = 0;

        public GadgeteerSensor(int id)
            : base(id)
        {
        }

        public float GetAnalogLightValue()
        {
            return this.lightvalue;
        }

        public override SensorData GetData()
        {
            var sensorData = new SensorData
                                 {
                                     SensorType = this, 
                                     ReadTime = DateTime.Now, 
                                     AnalogLight = this.GetAnalogLightValue()
                                 };
            return sensorData;
        }

        public void SetAnalogLightValue(float lightvalue)
        {
            this.lightvalue = lightvalue;
        }
    }
}
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

    using GHI.Premium.Net;

    using Microsoft.SPOT;

    using OccupOS.CommonLibrary.Sensors;

    public partial class Program
    {
        private readonly GT.Timer timer = new GT.Timer(2000);

        private GadgeteerWiFiNetworkController networkController;

        // This method is run when the mainboard is powered up or reset. 
        private void ProgramStarted()
        {
            DateTime time = new DateTime(2013, 3, 26, 13, 08, 00, 0);
            Microsoft.SPOT.Hardware.Utility.SetLocalTime(time);

            this.networkController = new GadgeteerWiFiNetworkController(wifi_RS21);
            this.networkController.ConnectToWiFi("testhoc", "1234567890");
            this.networkController.ConnectToSocket("192.168.1.52", 1333);

            this.timer.Tick += this.timer_Tick;
            this.timer.Start();

            Debug.Print("Finished setup");
        }

        private void timer_Tick(GT.Timer timer)
        {
            var sensorData = new SensorData();
            sensorData.AnalogLight = (int)this.lightSensor.ReadLightSensorPercentage();
            Debug.Print("Sending data: AnalogLight - " + sensorData.AnalogLight);

            string packet = PacketFactory.CreatePacket(sensorData);
            this.networkController.SendData(packet);

            Thread.Sleep(10000);
        }
    }
}
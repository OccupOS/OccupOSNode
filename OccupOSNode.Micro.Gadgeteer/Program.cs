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

using GT = Gadgeteer;

namespace GadgeteerDemo
{
    using System.Threading;

    using GHI.Premium.Net;

    using Microsoft.SPOT;
    using System;

    public partial class Program
    {
        #region Fields

        private readonly GT.Timer timer = new GT.Timer(2000);

        private NetworkController networkController;

        #endregion

        #region Methods

        // This method is run when the mainboard is powered up or reset. 
        private void ProgramStarted()
        {
            DateTime time = new DateTime(2013, 3, 26, 13, 08, 00, 0);
            Microsoft.SPOT.Hardware.Utility.SetLocalTime(time);

            this.wifi_RS21.DebugPrintEnabled = true;

            this.wifi_RS21.Interface.Open();

            NetworkInterfaceExtension.AssignNetworkingStackTo(this.wifi_RS21.Interface);

            this.wifi_RS21.Interface.NetworkInterface.EnableDhcp();
            this.wifi_RS21.Interface.NetworkInterface.EnableDynamicDns();

            // wifi_RS21.Interface.NetworkInterface.EnableStaticIP("192.168.1.202", "255.255.255.0", "192.168.12.1");

            Debug.Print("Scanning for WiFi networks");
            WiFiNetworkInfo[] wiFiNetworkInfo = this.wifi_RS21.Interface.Scan();
            if (wiFiNetworkInfo != null)
            {
                Debug.Print("Found WiFi network(s)");
                for (int i = 0; i < wiFiNetworkInfo.Length - 1; i++)
                {
                    if (wiFiNetworkInfo[i].SSID == "testhoc")
                    {
                        Debug.Print("Joining: " + wiFiNetworkInfo[i].SSID);
                        this.wifi_RS21.Interface.Join(wiFiNetworkInfo[i], "1234567890");
                    }
                    else
                    {
                        Debug.Print("Skipping: " + wiFiNetworkInfo[i].SSID);
                    }
                }

                Debug.Print(this.wifi_RS21.Interface.IsLinkConnected ? "Connection successful!" : "Connection failed!");
            }
            else
            {
                Debug.Print("Didn't find any WiFi networks");
            }

            this.networkController = new NetworkController("192.168.1.52", 1333);
            this.networkController.Connect();

            Debug.Print("Connected to socket!");

            this.timer.Tick += this.timer_Tick;
            this.timer.Start();

            Debug.Print("Finished setup");
        }

        private void timer_Tick(GT.Timer timer)
        {
            SensorData sensorData = new SensorData();
            sensorData.AnalogLight = (int) lightSensor.ReadLightSensorPercentage();
            Debug.Print("Sending data: AnalogLight - " + sensorData.AnalogLight);

            string packet = PacketFactory.CreatePacket(sensorData);
            networkController.SendData(packet);

            Thread.Sleep(10000);
        }

        #endregion
    }
}
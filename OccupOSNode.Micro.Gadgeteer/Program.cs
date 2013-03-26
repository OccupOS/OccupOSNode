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
    using OccupOS.CommonLibrary.Sensors;

    /// <summary>
    ///     The program.
    /// </summary>
    public partial class Program
    {
        #region Fields

        /// <summary>
        ///     The timer.
        /// </summary>
        private readonly GT.Timer timer = new GT.Timer(2000);

        /// <summary>
        ///     The network controller.
        /// </summary>
        private NetworkController networkController;

        #endregion

        // This method is run when the mainboard is powered up or reset.   
        #region Methods

        /// <summary>
        ///     The program started.
        /// </summary>
        private void ProgramStarted()
        {
            this.wifi_RS21.DebugPrintEnabled = true;

            this.wifi_RS21.Interface.Open();

            NetworkInterfaceExtension.AssignNetworkingStackTo(this.wifi_RS21.Interface);

            this.wifi_RS21.Interface.NetworkInterface.EnableDhcp();
            this.wifi_RS21.Interface.NetworkInterface.EnableDynamicDns();

            // wifi_RS21.Interface.NetworkInterface.EnableStaticIP("192.168.1.202", "255.255.255.0", "192.168.12.1");
            if (this.wifi_RS21.Interface.NetworkInterface.IsDhcpEnabled)
            {
                Debug.Print("DHCP enables");
            }

            Debug.Print("Scanning for WiFi networks");
            WiFiNetworkInfo[] wiFiNetworkInfo = this.wifi_RS21.Interface.Scan();
            if (wiFiNetworkInfo != null)
            {
                Debug.Print("Found WiFi network(s)");
                for (int i = 0; i < wiFiNetworkInfo.Length - 1; i++)
                {
                    if (wiFiNetworkInfo[i].SSID == "Pretty Fly for a Wi-Fi")
                    {
                        Debug.Print("Joining: " + wiFiNetworkInfo[i].SSID);
                        this.wifi_RS21.Interface.Join(wiFiNetworkInfo[i], "IWouldn'tBeSoStupidAsToCommitMyWiFiPasswordToSourceControlWouldI?");
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

            this.networkController = new NetworkController("192.168.1.68", 80);
            this.networkController.Connect();

            Debug.Print("Connected to socket!");

            this.timer.Tick += this.timer_Tick;
            this.timer.Start();

            Debug.Print("Finished setup");
        }

        /// <summary>
        /// The timer_ tick.
        /// </summary>
        /// <param name="timer">
        /// The timer.
        /// </param>
        private void timer_Tick(GT.Timer timer)
        {
            SensorData sensorData = new SensorData();
            sensorData.AnalogLight = (int) this.lightSensor.ReadLightSensorPercentage();

            this.networkController.SendData(sensorData.AnalogLight.ToString());
            Debug.Print("(Data sent: light percentage - " + sensorData.AnalogLight);

            Thread.Sleep(10000);
        }

        #endregion
    }
}
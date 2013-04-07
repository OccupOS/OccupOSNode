// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkController.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GadgeteerDemo
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    using Gadgeteer.Modules.GHIElectronics;

    using GHI.Premium.Net;

    using Microsoft.SPOT;

    using OccupOS.CommonLibrary.NetworkControllers;

    public class GadgeteerWiFiNetworkController : WirelessNetworkController
    {
        private Socket socket;
        private WiFi_RS21 wiFiSensor;

        public GadgeteerWiFiNetworkController(WiFi_RS21 wiFiSensor)
        {
            this.wiFiSensor = wiFiSensor;
        }

        public override void ConnectToSocket(string hostName, ushort port)
        {
            IPAddress hostAddress = IPAddress.Parse(hostName);
            IPEndPoint remoteEndPoint = new IPEndPoint(hostAddress, port);

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.Connect(remoteEndPoint);
            this.socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            this.socket.SendTimeout = 5000;
            Debug.Print("Connected to socket!");
        }

        public override void ConnectToWiFi(string SSID, string password)
        {
            this.wiFiSensor.DebugPrintEnabled = true;

            this.wiFiSensor.Interface.Open();

            NetworkInterfaceExtension.AssignNetworkingStackTo(this.wiFiSensor.Interface);

            this.wiFiSensor.Interface.NetworkInterface.EnableDhcp();
            this.wiFiSensor.Interface.NetworkInterface.EnableDynamicDns();

            Debug.Print("Scanning for WiFi networks");
            WiFiNetworkInfo[] wiFiNetworkInfo = this.wiFiSensor.Interface.Scan();
            if (wiFiNetworkInfo != null)
            {
                Debug.Print("Found WiFi network(s)");
                for (int i = 0; i < wiFiNetworkInfo.Length - 1; i++)
                {
                    if (wiFiNetworkInfo[i].SSID == SSID)
                    {
                        Debug.Print("Joining: " + wiFiNetworkInfo[i].SSID);
                        this.wiFiSensor.Interface.Join(wiFiNetworkInfo[i], password);
                    }
                    else
                    {
                        Debug.Print("Skipping: " + wiFiNetworkInfo[i].SSID);
                    }
                }

                Debug.Print(this.wiFiSensor.Interface.IsLinkConnected ? "Connection successful!" : "Connection failed!");
            }
            else
            {
                Debug.Print("Didn't find any WiFi networks");
            }
        }

        public override void DisconnectFromSocket()
        {
            throw new System.NotImplementedException();
        }

        public override void SendData(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            this.socket.Send(buffer);
        }
    }
}
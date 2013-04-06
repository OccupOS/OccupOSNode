// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetduinoWirelessNetworkController.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOSNode.Micro.NetworkControllers.Netduino
{
    using System;
    using System.Net.Sockets;
    using System.Text;

    using Microsoft.SPOT;

    using OccupOS.CommonLibrary.NetworkControllers;

    using Toolbox.NETMF.Hardware;
    using Toolbox.NETMF.NET;

    public class NetduinoWirelessNetworkController : WirelessNetworkController
    {
        private SimpleSocket socket = null;
        private WiFlyGSX wf_module;

        public NetduinoWirelessNetworkController()
        {
            this.wf_module = new WiFlyGSX();
        }



        public override void ConnectToWiFi(string SSID, string password)
        {
            this.wf_module.EnableDHCP();
            this.wf_module.JoinNetwork(SSID, 0, WiFlyGSX.AuthMode.MixedWPA1_WPA2, password);
        }

        public override void ConnectToSocket(string hostName, ushort port)
        {
            this.HostName = hostName;
            this.Port = port;

            this.socket = new WiFlySocket(hostName, port, this.wf_module);
            this.socket.Connect();
        }

        public override void DisconnectFromSocket()
        {
            this.HostName = default(string);
            this.Port = default(ushort);

            this.wf_module.CloseSocket();
            this.socket.Close();
            this.socket = null;
        }

        public override void SendData(string data)
        {
            if (this.socket == null || data == null)
            {
                throw new NullReferenceException();
            }

            byte[] cmdBytes = Encoding.UTF8.GetBytes(data + "\r\n");
            this.socket.SendBinary(cmdBytes);
        }
    }
}
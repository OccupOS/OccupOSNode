// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FullEthernetController.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("OccupOSNode.Tests")]

namespace OccupOSNode.NetworkControllers
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    using OccupOS.CommonLibrary.NetworkControllers;

    internal class FullEthernetNetworkController : EthernetNetworkController
    {
        private Socket socket;

        public FullEthernetNetworkController() : base(null, 0) { }

        public FullEthernetNetworkController(string Hostname, ushort Port) : base(Hostname, Port) { }

        public override void ConnectToSocket(string hostName, ushort port)
        {
            this.HostName = hostName;
            this.Port = port;

            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            IPAddress hostAddress = hostEntry.AddressList[0];
            try
            {
                hostAddress = IPAddress.Parse(hostName);
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is FormatException)
                {
                    hostAddress = Dns.GetHostAddresses(hostName)[0];
                }
            }

            IPEndPoint remoteEndPoint = new IPEndPoint(hostAddress, port);
            
            this.socket = new Socket(hostAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.socket.Connect(remoteEndPoint);
            this.socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            this.socket.SendTimeout = 5000;
        }

        public override void DisconnectFromSocket()
        {
            this.HostName = default(string);
            this.Port = default(ushort);

            this.socket.Close();
            this.socket = null;
        }

        public override void SendData(string data)
        {
            if (this.socket == null || data == null)
            {
                throw new NullReferenceException();
            }
            
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                this.socket.Send(buffer);
            } 
            catch (ObjectDisposedException e)
            {
                this.socket = null;
                throw new NullReferenceException();
            }
        }
    }
}
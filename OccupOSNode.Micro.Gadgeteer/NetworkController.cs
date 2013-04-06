// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkController.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   This is NOT supposed to reside in the OccupOSNode.Micro.Gadgeteer project. 
//   It is a temporary hack until Gadgeteer supports the .NET MF 4.3 Framework.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GadgeteerDemo
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class NetworkController
    {
        private readonly IPAddress hostAddress;
        private readonly IPEndPoint remoteEndPoint;
        private string address;
        private IPHostEntry hostEntry;
        private Socket socket;

        public NetworkController(string hostName, int port)
        {
            this.address = hostName;

            // hostEntry = Dns.GetHostEntry(hostName);
            // hostAddress = hostEntry.AddressList[0];
            this.hostAddress = IPAddress.Parse(hostName);
            this.remoteEndPoint = new IPEndPoint(this.hostAddress, port);
        }

        public Socket Connect()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.Connect(this.remoteEndPoint);
            this.socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            this.socket.SendTimeout = 5000;
            return this.socket;
        }

        public int SendData(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            return this.socket.Send(buffer);
        }
    }
}
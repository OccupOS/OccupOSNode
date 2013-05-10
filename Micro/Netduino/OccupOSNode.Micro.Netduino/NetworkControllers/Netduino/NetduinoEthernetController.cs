// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetduinoEthernetController.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace OccupOSNode.Micro.NetworkControllers.Netduino
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    using OccupOS.CommonLibrary.NetworkControllers;

    public class NetduinoEthernetController : EthernetNetworkController
    {
        private Socket socket;

        public NetduinoEthernetController()
            : base(null, 0)
        {
        }

        public NetduinoEthernetController(string Hostname, ushort Port)
            : base(Hostname, Port)
        {
        }

        public static bool UpdateTimeFromNtpServer(string server, int timeZoneOffset)
        {
            try
            {
                var currentTime = GetNtpTime(server, timeZoneOffset);
                Microsoft.SPOT.Hardware.Utility.SetLocalTime(currentTime);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override void ConnectToSocket(string hostName, ushort port)
        {
            this.HostName = hostName;
            this.Port = port;

            IPAddress hostAddress;
            try
            {
                hostAddress = IPAddress.Parse(hostName);
            }
            catch (ArgumentException e)
            {
                IPAddress[] list = Dns.GetHostEntry(hostName).AddressList;
                hostAddress = Dns.GetHostEntry(hostName).AddressList[0];
            }

            IPEndPoint remoteEndPoint = new IPEndPoint(hostAddress, port);

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.Connect(remoteEndPoint);
            this.socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            this.socket.SendTimeout = 5000;
        }

        public override void DisconnectFromSocket()
        {
            this.HostName = default(string);
            this.Port = default(ushort);

            if (this.socket != null)
            {
                this.socket.Close();
                this.socket = null;
            }
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

        /*Following two methods by Michael Schwarz, 
         * http://weblogs.asp.net/mschwarz/archive/2008/03/09/wrong-datetime-on-net-micro-framework-devices.aspx
         */
        private static DateTime GetNtpTime(string timeServer, int timeZoneOffset)
        {
            var ep = new IPEndPoint(Dns.GetHostEntry(timeServer).AddressList[0], 123);
            var ntpData = new byte[48];
            using (var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                s.SendTimeout = s.ReceiveTimeout = 10000;
                s.Connect(ep);
                ntpData[0] = 0x1B;
                s.Send(ntpData);
                s.Receive(ntpData);
                s.Close();
            }

            const byte offsetTransmitTime = 40;

            ulong intpart = 0;
            ulong fractpart = 0;

            for (var i = 0; i <= 3; i++)
            {
                intpart = (intpart << 8) | ntpData[offsetTransmitTime + i];
            }

            for (var i = 4; i <= 7; i++)
            {
                fractpart = (fractpart << 8) | ntpData[offsetTransmitTime + i];
            }

            ulong milliseconds = intpart * 1000 + (fractpart * 1000) / 0x100000000L;

            var timeSpan = TimeSpan.FromTicks((long)milliseconds * TimeSpan.TicksPerMillisecond);
            var dateTime = new DateTime(1900, 1, 1);
            dateTime += timeSpan;

            var offsetAmount = new TimeSpan(timeZoneOffset, 0, 0);
            var networkDateTime = dateTime + offsetAmount;

            return networkDateTime;
        }
    }
}
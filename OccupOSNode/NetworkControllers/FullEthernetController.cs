// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FullEthernetController.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOSNode.NetworkControllers
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    internal class FullEthernetController : OccupOS.CommonLibrary.NetworkControllers.NetworkController
    {
        #region Fields

        private Socket socket;

        #endregion

        #region Constructors and Destructors

        public FullEthernetController(string hostname, ushort port)
            : base(hostname, port)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool Connect(string SSID, string key)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(this.hostname);
            IPAddress hostAddress = hostEntry.AddressList[0];
            try
            {
                hostAddress = IPAddress.Parse(this.hostname);
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is FormatException)
                {
                    hostAddress = Dns.GetHostAddresses(this.hostname)[0];
                }
            }

            IPEndPoint remoteEndPoint = new IPEndPoint(hostAddress, this.port);
            this.socket = new Socket(hostAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                this.socket.Connect(remoteEndPoint);
            }
            catch (SocketException e)
            {
                return false;
            }

            this.socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            this.socket.SendTimeout = 5000;
            return true;
        }

        public override void Disconnect()
        {
            this.socket.Close();
            this.socket = null;
        }

        public override int SendData(string data)
        {
            if (this.socket != null)
            {
                try
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    return this.socket.Send(buffer);
                }
                catch (Exception e)
                {
                    if (e is SocketException || e is ObjectDisposedException)
                    {
                        this.socket = null;
                    }

                    return 0;
                }
            }

            return 0;
        }

        #endregion
    }
}
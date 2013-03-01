// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkController.cs" company="OccupOS">
//   This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License along with this program. If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The network controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GadgeteerDemo
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// The network controller.
    /// </summary>
    public class NetworkController
    {
        #region Fields

        /// <summary>
        /// The host address.
        /// </summary>
        private readonly IPAddress hostAddress;

        /// <summary>
        /// The remote end point.
        /// </summary>
        private readonly IPEndPoint remoteEndPoint;

        /// <summary>
        /// The address.
        /// </summary>
        private string address;

        /// <summary>
        /// The host entry.
        /// </summary>
        private IPHostEntry hostEntry;

        /// <summary>
        /// The socket.
        /// </summary>
        private Socket socket;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="NetworkController"/> class.
        /// </summary>
        /// <param name="hostName">
        /// The host name.
        /// </param>
        /// <param name="port">
        /// The port.
        /// </param>
        public NetworkController(string hostName, int port)
        {
            this.address = hostName;

            // hostEntry = Dns.GetHostEntry(hostName);
            // hostAddress = hostEntry.AddressList[0];
            this.hostAddress = IPAddress.Parse(hostName);
            this.remoteEndPoint = new IPEndPoint(this.hostAddress, port);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The connect.
        /// </summary>
        /// <returns>
        /// The <see cref="Socket"/>.
        /// </returns>
        public Socket Connect()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.Connect(this.remoteEndPoint);
            this.socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            this.socket.SendTimeout = 5000;
            return this.socket;
        }

        /// <summary>
        /// The send data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SendData(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            return this.socket.Send(buffer);
        }

        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkController.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace OccupOS.CommonLibrary.NetworkControllers
{
    using System;

    public abstract class NetworkController
    {
        protected NetworkController(string hostName, ushort port)
        {
            this.HostName = hostName;
            this.Port = port;
        }

        public string HostName { get; protected set; }

        public ushort Port { get; protected set; }

        public abstract void ConnectToSocket(string hostName, ushort port);

        public void ConnectToSocket()
        {
            if (this.HostName != null)
            {
                this.ConnectToSocket(this.HostName, this.Port);
            }
            else
            {
                throw new ArgumentNullException("Default HostName and Port arguments have not specified");
            }
        }

        public abstract void DisconnectFromSocket();

        public abstract void SendData(string data);
    }
}
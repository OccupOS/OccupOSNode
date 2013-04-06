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

    public abstract class WirelessNetworkController : NetworkController
    {
        public WirelessNetworkController(string HostName, ushort Port, string SSID, string Password) : base(HostName, Port)
        {
                this.SSID = SSID;
                this.Password = Password;
        }

        public WirelessNetworkController() : base(null, 0) {
            this.SSID = null;
            this.Password = null;
        }

        public string SSID { get; protected set; }

        public string Password { get; protected set; }

        public abstract void ConnectToWiFi(string SSID, string Password);

        public void ConnectToWiFi() {
            if (SSID != null && Password != null)
                ConnectToWiFi(SSID, Password);
            else
                throw new ArgumentNullException("Default HostName and Port arguments have not specified");
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOSNode.Micro
{
    using System;

    using Microsoft.SPOT.Hardware;

    using OccupOS.CommonLibrary;
    using OccupOS.CommonLibrary.Sensors;
    using OccupOS.CommonLibrary.NodeControllers;
    using OccupOSNode.Micro.HardwareControllers.Netduino;
    using OccupOSNode.Micro.NetworkControllers.Netduino;
    using SecretLabs.NETMF.Hardware.NetduinoPlus;
    using System.Threading;

    public class Program
    {
        public static void Main()
        {
            //var networkController = new NetduinoWirelessNetworkController("192.168.0.3", 1333, "virginmedia6963974", "cssuvjcs");
            var networkController = new NetduinoEthernetController("192.168.0.3", 1333);
            NetduinoEthernetController.UpdateTimeFromNtpServer("time.nist.gov", 1); 
            var controller = new NetduinoNodeController(0, new NetduinoHardwareController(), networkController);
            controller.EnableDynamicListening();
            //Thread.Sleep(10000);
            //controller.DisableDynamicListening();
            controller.Start(60000, 30000, 40);
        }
    }
}
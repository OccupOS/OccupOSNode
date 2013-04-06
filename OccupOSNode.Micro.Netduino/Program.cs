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

    using OccupOSNode.Micro.HardwareControllers.Netduino;
    using OccupOSNode.Micro.NetworkControllers.Netduino;

    using SecretLabs.NETMF.Hardware.NetduinoPlus;

    public class Program
    {
        private static readonly OutputPort outPrt = new OutputPort(Pins.ONBOARD_LED, false);

        public static void Main()
        {
            var networkController = new NetduinoWirelessNetworkController();
            networkController.ConnectToWiFi("RichyHotspot", "occupos8");
            networkController.ConnectToSocket("UrsaMinor", 1333);

            var controller = new NetduinoNodeController(0, new NetduinoHardwareController(), networkController);
            controller.EnableDynamicListening();

            var sensorData = new SensorData();
            while (true)
            {
                int sensors = 0; /*controller.GetSensorCount();*/
                if (sensors == 1)
                {
                    var data = new SensorData();
                        
                        /*((NetduinoWeatherShieldSensor)controller.GetSensor(0)).GetData();*/
                    try
                    {
                        networkController.SendData(PacketFactory.CreatePacket(data));
                    }
                    catch (Exception e)
                    {
                        networkController.ConnectToWiFi("RichyHotspot", "occupos8");
                    }

                    // string jsontest = PacketFactory.SerializeJSON(0, new SensorData[] {data});
                }

                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetduinoWeatherShieldSensor.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOSNode.Micro.Sensors.Netduino
{
    using System;

    using OccupOS.CommonLibrary.Sensors;

    using SecretLabs.NETMF.Hardware.NetduinoPlus;

    internal class NetduinoWeatherShieldSensor : Sensor, 
                                                 IHumiditySensor, 
                                                 IPressureSensor, 
                                                 ITemperatureSensor, 
                                                 IDynamicSensor
    {
        private readonly NetduinoWeatherShieldDriver driver;

        private byte[] data;

        private float humidity;

        private float pressure;

        private float temp;

        public NetduinoWeatherShieldSensor(int id)
            : base(id)
        {
            this.driver = new NetduinoWeatherShieldDriver(
                Pins.GPIO_PIN_D7, Pins.GPIO_PIN_D2, NetduinoWeatherShieldDriver.DEFAULTADDRESS);
            this.data = new byte[4];
        }

        public void Connect()
        {
        }

        public void Disconnect()
        {
        }

        public ConnectionStatus GetConnectionStatus()
        {
            return this.driver.echo(0x55) == 0x55 ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
        }

        public override SensorData GetData()
        {
            var sensorData = new SensorData
                                 {
                                     SensorType = this, 
                                     ReadTime = DateTime.Now, 
                                     Humidity = this.GetHumidity(), 
                                     Pressure = this.GetPressure(), 
                                     Temperature = this.GetTemperature()
                                 };
            return sensorData;
        }

        public int GetDeviceCount()
        {
            // Maximum of 1 device per Netduino node
            return this.GetConnectionStatus() == ConnectionStatus.Connected ? 1 : 0;
        }

        public float GetHumidity()
        {
            this.humidity = this.driver.readAveragedValue(NetduinoWeatherShieldDriver.units.HUMIDITY);
            return this.humidity;
        }

        public int GetMaxSensors()
        {
            return 1;
        }

        public float GetPressure()
        {
            this.pressure = this.driver.readAveragedValue(NetduinoWeatherShieldDriver.units.PRESSURE);
            return this.pressure;
        }

        public float GetTemperature()
        {
            this.temp = this.driver.readAveragedValue(NetduinoWeatherShieldDriver.units.TEMPERATURE);
            return this.temp;
        }
    }
}
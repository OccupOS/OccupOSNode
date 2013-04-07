using System;
using OccupOS.CommonLibrary.Sensors;

namespace OccupOSNode.Micro.Sensors.Netduino {
    using SecretLabs.NETMF.Hardware.NetduinoPlus;

    internal class NetduinoWeatherShieldSensor : Sensor, IHumiditySensor, IPressureSensor, ITemperatureSensor, IDynamicSensor {
        private readonly NetduinoWeatherShieldDriver driver;
        private byte[] data;

        private float humidity, pressure, temp;

        public NetduinoWeatherShieldSensor(int id)
            : base(id) {
                driver = new NetduinoWeatherShieldDriver(Pins.GPIO_PIN_D7, Pins.GPIO_PIN_D2, NetduinoWeatherShieldDriver.DEFAULTADDRESS);
            data = new byte[4];
        }

        public float GetHumidity()
        {
            humidity = driver.readAveragedValue(NetduinoWeatherShieldDriver.units.HUMIDITY);
            return humidity;
        }

        public float GetPressure()
        {
            pressure = driver.readAveragedValue(NetduinoWeatherShieldDriver.units.PRESSURE);
            return pressure;
        }

        public float GetTemperature()
        {
            temp = driver.readAveragedValue(NetduinoWeatherShieldDriver.units.TEMPERATURE);
            return temp;
        }

        public override SensorData GetData() {
            var sensorData = new SensorData {
                Sensorobj = this,
                ReadTime = DateTime.Now,
                Humidity = GetHumidity(),
                Pressure = GetPressure(),
                Temperature = GetTemperature()
            };
            return sensorData;
        }

        public ConnectionStatus GetConnectionStatus() {
            return driver.echo(0x55) == 0x55 ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
        }

        public int GetMaxSensors() {
            return 1;
        }

        public int GetDeviceCount() {
            //Maximum of 1 device per Netduino node
            return GetConnectionStatus() == ConnectionStatus.Connected ? 1 : 0;
            }

        //no additional dynamic connection/disconnection actions are necessary
        public void Connect() { }
        public void Disconnect() { }
    }
}
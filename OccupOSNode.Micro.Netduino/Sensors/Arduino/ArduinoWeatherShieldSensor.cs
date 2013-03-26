using System;
using OccupOS.CommonLibrary.Sensors;

namespace OccupOSNode.Micro.Sensors.Arduino {
    using SecretLabs.NETMF.Hardware.NetduinoPlus;

    internal class ArduinoWeatherShieldSensor : Sensor, IHumiditySensor, IPressureSensor, ITemperatureSensor, IDynamicSensor {
        private readonly ArduinoWeatherShieldDriver driver;
        private byte[] data;

        private float humidity, pressure, temp;

        public ArduinoWeatherShieldSensor(int id)
            : base(id) {
                driver = new ArduinoWeatherShieldDriver(Pins.GPIO_PIN_D7, Pins.GPIO_PIN_D2, ArduinoWeatherShieldDriver.DEFAULTADDRESS);
            data = new byte[4];
        }

        public float GetHumidity()
        {
            humidity = driver.readAveragedValue(ArduinoWeatherShieldDriver.units.HUMIDITY);
            return humidity;
        }

        public float GetPressure()
        {
            pressure = driver.readAveragedValue(ArduinoWeatherShieldDriver.units.PRESSURE);
            return pressure;
        }

        public float GetTemperature()
        {
            temp = driver.readAveragedValue(ArduinoWeatherShieldDriver.units.TEMPERATURE);
            return temp;
        }

        public override SensorData GetData() {
            var sensorData = new SensorData {
                Humidity = GetHumidity(),
                Pressure = GetPressure(),
                Temperature = GetTemperature()
            };
            return sensorData;
        }

        public ConnectionStatus GetConnectionStatus() {
            return driver.echo(0x55) == 0x55 ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
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
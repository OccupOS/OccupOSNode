using System;
using OccupOS.CommonLibrary.Sensors;

namespace OccupOSNode.Micro.Sensors.Arduino {
    using SecretLabs.NETMF.Hardware.NetduinoPlus;

    internal class ArduinoWeatherShield1Sensor : Sensor, IHumiditySensor, IPressureSensor, ITemperatureSensor {
        private readonly ArduinoWeatherShield1Controller controller;
        private byte[] data;

        private float humidity, pressure, temp;

        public ArduinoWeatherShield1Sensor(int id)
            : base(id) {
                controller = new ArduinoWeatherShield1Controller(Pins.GPIO_PIN_D7, Pins.GPIO_PIN_D2, ArduinoWeatherShield1Controller.DEFAULTADDRESS);
            data = new byte[4];
        }

        public float GetHumidity()
        {
            humidity = controller.readAveragedValue(ArduinoWeatherShield1Controller.units.HUMIDITY);
            return humidity;
        }

        public float GetPressure()
        {
            pressure = controller.readAveragedValue(ArduinoWeatherShield1Controller.units.PRESSURE);
            return pressure;
        }

        public float GetTemperature()
        {
            temp = controller.readAveragedValue(ArduinoWeatherShield1Controller.units.TEMPERATURE);
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
    }
}
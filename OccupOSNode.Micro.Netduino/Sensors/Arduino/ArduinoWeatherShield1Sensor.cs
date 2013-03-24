﻿using System;
using OccupOS.CommonLibrary.Sensors;

namespace OccupOSNode.Micro.Sensors.Arduino {
    internal class ArduinoWeatherShield1Sensor : Sensor, IHumiditySensor, IPressureSensor, ITemperatureSensor {
        private readonly ArduinoWeatherShield1Controller controller;
        private byte[] data;

        public ArduinoWeatherShield1Sensor(int id)
            : base(id) {
            controller = new ArduinoWeatherShield1Controller();
            data = new byte[4];
        }

        public float GetHumidity() {
            return controller.sendCommand(ArduinoWeatherShield1Controller.CMD_GETHUM_RAW,
                                          ArduinoWeatherShield1Controller.PAR_GET_LAST_SAMPLE,
                                          ref data)
                       ? controller.decodeShortValue(data)
                       : 0f;
        }

        public float GetPressure() {
            return controller.sendCommand(ArduinoWeatherShield1Controller.CMD_GETPRESS_RAW,
                                          ArduinoWeatherShield1Controller.PAR_GET_LAST_SAMPLE,
                                          ref data)
                       ? controller.decodeShortValue(data)
                       : 0f;
        }

        public float GetTemperature() {
            return controller.sendCommand(ArduinoWeatherShield1Controller.CMD_GETTEMP_C_RAW,
                                          ArduinoWeatherShield1Controller.PAR_GET_LAST_SAMPLE,
                                          ref data)
                       ? controller.decodeShortValue(data)
                       : 0f;
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
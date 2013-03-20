using System;
using System.Collections;
using Microsoft.SPOT.Hardware;
using OccupOS.CommonLibrary.Sensors;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace OccupOSNode.Micro.Sensors.Arduino {
    internal class LightSensor : Sensor, ILightSensor {
        private readonly AnalogInput input;
        private readonly Hashtable ports = new Hashtable();
        private float analogValue, digitalValue;

        public LightSensor(int id, int portNumber)
            : base(id) {
            Setup();

            if (portNumber > 0 &&
                portNumber < 6) {
                input = new AnalogInput((Cpu.AnalogChannel)ports[portNumber]);
            }
            else {
                input = new AnalogInput((Cpu.AnalogChannel)ports[0]);
            }
        }

        public float GetAnalogLightValue() {
            digitalValue = (float)input.Read();
            analogValue = (float)(digitalValue / 1023 * 3.3);
            return analogValue;
        }

        public override SensorData GetData() {
            var sensorData = new SensorData { AnalogLight = GetAnalogLightValue() };
            return sensorData;
        }

        private void Setup() {
            ports.Add(0, Pins.GPIO_PIN_A0);
            ports.Add(1, Pins.GPIO_PIN_A1);
            ports.Add(2, Pins.GPIO_PIN_A2);
            ports.Add(3, Pins.GPIO_PIN_A3);
            ports.Add(4, Pins.GPIO_PIN_A4);
            ports.Add(5, Pins.GPIO_PIN_A5);
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetduinoAmbientLightSensor.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOSNode.Micro.Sensors.Netduino
{
    using System.Collections;

    using Microsoft.SPOT.Hardware;

    using OccupOS.CommonLibrary.Sensors;

    using SecretLabs.NETMF.Hardware.NetduinoPlus;

    internal class LightSensor : Sensor, ILightSensor
    {
        private readonly AnalogInput input;

        private readonly Hashtable ports = new Hashtable();

        private float analogValue;

        private float digitalValue;

        public LightSensor(int id, int portNumber)
            : base(id)
        {
            this.Setup();

            if (portNumber > 0 && portNumber < 6)
            {
                this.input = new AnalogInput((Cpu.AnalogChannel)this.ports[portNumber]);
            }
            else
            {
                this.input = new AnalogInput((Cpu.AnalogChannel)this.ports[0]);
            }
        }

        public float GetAnalogLightValue()
        {
            this.digitalValue = (float)this.input.Read();
            this.analogValue = (float)(this.digitalValue / 1023 * 3.3);
            return this.analogValue;
        }

        public override SensorData GetData()
        {
            var sensorData = new SensorData { AnalogLight = this.GetAnalogLightValue() };
            return sensorData;
        }

        private void Setup()
        {
            this.ports.Add(0, Pins.GPIO_PIN_A0);
            this.ports.Add(1, Pins.GPIO_PIN_A1);
            this.ports.Add(2, Pins.GPIO_PIN_A2);
            this.ports.Add(3, Pins.GPIO_PIN_A3);
            this.ports.Add(4, Pins.GPIO_PIN_A4);
            this.ports.Add(5, Pins.GPIO_PIN_A5);
        }
    }
}
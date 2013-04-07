// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SensorPoller.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOS.CommonLibrary.HardwareControllers
{
    using System;
    using System.Collections;
    using OccupOS.CommonLibrary.Sensors;

    public class SensorPoller
    {
        private int delay_time;
        private HardwareController hw_controller;
        private int max_buffer_size;

        public SensorPoller(HardwareController hardwareController, int delay, int maxBuffer)
        {
            if (delay < 0 || maxBuffer < 1)
            {
                throw new ArgumentException();
            }

            this.hw_controller = hardwareController;
            this.delay_time = delay;
            this.max_buffer_size = maxBuffer;
        }

        public void Run()
        {
            while (true)
            {
                if (this.hw_controller.GetSensorDataBufferCount() >= this.max_buffer_size)
                {
                    this.hw_controller.RemoveSensorReadings(0);
                }
                SensorData[] readings = SampleSensorData();
                if (readings != null)
                    this.hw_controller.AddSensorReadings(readings);
                System.Threading.Thread.Sleep(this.delay_time);
            }
        }

        private SensorData[] SampleSensorData()
        {
            ArrayList sample = this.hw_controller.GetAllSensors();
            SensorData[] result = null;
            if (sample != null) {
                result = new SensorData[sample.Count];
                for (int k = 0; k < sample.Count; k++) {
                    result[k] = ((Sensor)sample[k]).GetData();
                }
            }
            return result;
        }
    }
}
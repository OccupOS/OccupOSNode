// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareController.cs" company="OccupOS">
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

    public class HardwareController
    {
        private readonly ArrayList sensorDataBuffer;
        private readonly ArrayList sensors;

        public HardwareController()
        {
            this.sensorDataBuffer = new ArrayList();
            this.sensors = new ArrayList();
        }

        public void AddSensor(Sensor sensor)
        {
            if (sensor == null)
            {
                throw new NullReferenceException();
            }

            this.sensors.Add(sensor);
        }

        public void AddSensorReadings(SensorData[] data)
        {
            if (data != null && data.Length > 0)
            {
                this.sensorDataBuffer.Add(data);
            }
        }

        public ArrayList GetAllSensors()
        {
            var matches = new ArrayList();

            foreach (object sensor in this.sensors)
            {
                if (sensor is Sensor)
                {
                    matches.Add(sensor);
                }
            }

            return matches;
        }

        public ArrayList GetAllSensors(Type stype)
        {
            var matches = new ArrayList();

            foreach (object sensor in this.sensors)
            {
                if (sensor is Sensor)
                {
                    if (sensor.GetType() == stype)
                    {
                        matches.Add(sensor);
                    }
                }
            }

            return matches;
        }

        public Sensor GetSensor(int index)
        {
            if (index < 0 || index > this.sensors.Count - 1)
            {
                throw new IndexOutOfRangeException();
            }

            if (this.sensors[index] is Sensor)
            {
                return (Sensor)this.sensors[index];
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public int GetSensorCount()
        {
            return this.sensors.Count;
        }

        public int GetSensorCount(Type stype)
        {
            int count = 0;
            foreach (object sensor in this.sensors)
            {
                if (sensor is Sensor)
                {
                    if (sensor.GetType() == stype)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public int GetSensorDataBufferCount()
        {
            return this.sensorDataBuffer.Count;
        }

        public SensorData[] GetSensorReadings(int index)
        {
            if (index <= this.sensorDataBuffer.Count - 1)
            {
                if (this.sensorDataBuffer[index] is SensorData[])
                {
                    return (SensorData[])this.sensorDataBuffer[index];
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public SensorData[] PollSensorReadings(int index)
        {
            if (index <= this.sensorDataBuffer.Count - 1)
            {
                if (this.sensorDataBuffer[index] is SensorData[])
                {
                    var data = (SensorData[])this.sensorDataBuffer[index];
                    this.sensorDataBuffer.RemoveAt(index);
                    return data;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public void RemoveSensor(int index)
        {
            if (index <= this.sensors.Count - 1)
            {
                this.sensors.RemoveAt(index);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public void RemoveSensorByID(int id)
        {
            int sensornum = 0;
            for (int k = 0; k < this.sensors.Count; k++)
            {
                var sensor = this.sensors[sensornum];
                if (sensor is Sensor)
                {
                    if (id == ((Sensor)sensor).ID)
                    {
                        this.sensors.Remove(sensor);
                        sensornum--;
                    }
                }

                sensornum++;
            }
        }

        public void RemoveSensorReadings(int index)
        {
            if (index <= this.sensorDataBuffer.Count - 1)
            {
                this.sensorDataBuffer.RemoveAt(index);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}
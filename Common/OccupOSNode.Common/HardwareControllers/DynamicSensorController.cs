// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicSensorController.cs" company="OccupOS">
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
    using System.Reflection;
    using System.Threading;

    using OccupOS.CommonLibrary.Sensors;

    public class DynamicSensorController
    {
        private Assembly assembly;

        private ManualResetEvent eventWaiter = new ManualResetEvent(false);

        private HardwareController hardwareController = null;

        public DynamicSensorController(HardwareController hardwareController, Assembly assembly)
        {
            this.Enabled = false;
            this.hardwareController = hardwareController;
            this.assembly = assembly;
        }

        public bool Enabled { get; private set; }

        public void Disable()
        {
            this.Enabled = false;
        }

        public void Enable()
        {
            this.Enabled = true;
            this.eventWaiter.Set();
        }

        public void Run()
        {
            while (true)
            {
                if (!this.Enabled)
                {
                    this.eventWaiter.WaitOne();
                }
                else
                {
                    this.UpdateDynamicSensors();
                }

                Thread.Sleep(500);
            }
        }

        private void CreateSensor(Type stype, int count)
        {
            for (int k = 0; k < count; k++)
            {
                ConstructorInfo constructor = stype.GetConstructor(new[] { typeof(int) });
                if (constructor != null)
                {
                    Sensor newsensor =
                        constructor.Invoke(
                            new object[]
                                {
                                   this.FindLowestNumID(this.hardwareController.GetAllSensors(stype), stype, 0) 
                                }) as
                        Sensor;
                    this.hardwareController.AddSensor(newsensor);
                    if (newsensor is IDynamicSensor)
                    {
                        ((IDynamicSensor)newsensor).Connect();
                    }
                }
            }
        }

        private void DisposeInactives(Type stype)
        {
            ArrayList actives = this.hardwareController.GetAllSensors(stype);
            foreach (Sensor currentActive in actives)
            {
                try
                {
                    if (currentActive is IDynamicSensor)
                    {
                        if (((IDynamicSensor)currentActive).GetConnectionStatus() == ConnectionStatus.Disconnected)
                        {
                            this.DisposeSensor(currentActive);
                        }
                    }
                }
                catch (SensorNotFoundException)
                {
                    this.DisposeSensor(currentActive);
                }
            }
        }

        private void DisposeSensor(Sensor sensor)
        {
            if (sensor is IDynamicSensor)
            {
                ((IDynamicSensor)sensor).Disconnect();
            }

            this.hardwareController.RemoveSensorByID(sensor.ID);
        }

        private int FindLowestNumID(ArrayList sensorlist, Type stype, int startID)
        {
            // This method preferably needs to check whole database instead
            if (sensorlist.Count > 0)
            {
                foreach (Sensor currentActive in sensorlist)
                {
                    if (startID.ToString().Equals(currentActive.ID.ToString()))
                    {
                        this.FindLowestNumID(sensorlist, stype, startID + 1);
                    }
                }
            }

            return startID;
        }

        private void UpdateDynamicSensors()
        {
            foreach (var type in this.assembly.GetTypes())
            {
                if (type.IsClass)
                {
                    foreach (var iface in type.GetInterfaces())
                    {
                        if (iface.Name.Equals("IDynamicSensor"))
                        {
                            ConstructorInfo constructor = type.GetConstructor(new[] { typeof(int) });
                            if (constructor != null)
                            {
                                IDynamicSensor dsensor = constructor.Invoke(new object[] { -1 }) as IDynamicSensor;
                                int sensors_store = this.hardwareController.GetSensorCount(type);
                                int max_sensors = dsensor.GetMaxSensors();
                                if (sensors_store < max_sensors || max_sensors < 0)
                                {
                                    int sensorsConnected = dsensor.GetDeviceCount();
                                    if (sensorsConnected > sensors_store)
                                    {
                                        this.CreateSensor(type, sensorsConnected - sensors_store);
                                    }
                                    else
                                    {
                                        if (sensorsConnected < sensors_store)
                                        {
                                            this.DisposeInactives(type);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
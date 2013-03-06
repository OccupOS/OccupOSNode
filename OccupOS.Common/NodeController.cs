using System;
using System.Collections;
using OccupOS.CommonLibrary.Sensors;
using System.Reflection;

namespace OccupOS.CommonLibrary.NodeControllers {
    public class StorageDeviceMissingException : Exception {
        public StorageDeviceMissingException(string message)
            : base(message) { }
    }

    public abstract class NodeController {
        private readonly ArrayList sensorDataBuffer = new ArrayList();
        private readonly ArrayList sensors = new ArrayList();

        public int CheckForSensors() {
            int test = -1;
            foreach (var type in Assembly.GetAssembly(this.GetType()).GetTypes()) {
                if (type.IsClass) {
                    foreach (var iface in type.GetInterfaces()) {
                        if (iface.Name.Equals("IDynamicSensor")) {
                            ConstructorInfo constructor = type.GetConstructor(new Type[] {typeof(String)});
                            if (constructor != null) {
                                IDynamicSensor dsensor = constructor.Invoke(new Object[] {"test"}) as IDynamicSensor;
                                test = dsensor.GetDeviceCount();
                            }
                        }
                    }
                }
            }
            return test;
        }

        public void AddSensor(Sensor sensor) {
            if (sensor != null) {
                sensors.Add(sensor);
            }
        }

        public Sensor GetSensor(int index) {
            if (index <= sensors.Count - 1) {
                if (sensors[index] is Sensor) {
                    return (Sensor)sensors[index];
                }
                else {
                    throw new ArgumentNullException();
                }
            }
            else {
                throw new IndexOutOfRangeException();
            }
        }

        public void RemoveSensor(String id) {
            foreach (object sensor in sensors) {
                if (sensor is Sensor) {
                    if (id == ((Sensor)sensor).ID) {
                        sensors.Remove(sensor);
                    }
                }
            }
        }

        public void RemoveSensor(int index) {
            if (index <= sensors.Count - 1) {
                sensors.RemoveAt(index);
            }
            else {
                throw new IndexOutOfRangeException();
            }
        }

        public int GetSensorCount() { return sensors.Count; }

        public void AddSensorReading(SensorData data) {
            if (data != null) {
                sensorDataBuffer.Add(data);
            }
        }

        public SensorData GetSensorReading(int index) {
            if (index <= sensorDataBuffer.Count - 1) {
                if (sensorDataBuffer[index] is SensorData) {
                    return (SensorData)sensorDataBuffer[index];
                }
                else {
                    throw new ArgumentNullException();
                }
            }
            else {
                throw new IndexOutOfRangeException();
            }
        }

        public SensorData PollSensorReading(int index) {
            if (index <= sensorDataBuffer.Count - 1) {
                if (sensorDataBuffer[index] is SensorData) {
                    var data = (SensorData)sensorDataBuffer[index];
                    sensorDataBuffer.RemoveAt(index);
                    return data;
                }
                else {
                    throw new ArgumentNullException();
                }
            }
            else {
                throw new IndexOutOfRangeException();
            }
        }

        public void RemoveSensorReading(int index) {
            if (index <= sensorDataBuffer.Count - 1) {
                sensorDataBuffer.RemoveAt(index);
            }
            else {
                throw new IndexOutOfRangeException();
            }
        }

        public int GetSensorDataBufferCount() { return sensorDataBuffer.Count; }

        protected abstract void LoadConfiguration();
    }
}
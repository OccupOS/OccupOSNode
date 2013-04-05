namespace OccupOS.CommonLibrary.HardwareControllers {

    using System;
    using System.Collections;
    using System.Reflection;
    using OccupOS.CommonLibrary.Sensors;

    public class StorageDeviceMissingException : Exception {
        public StorageDeviceMissingException(string message)
            : base(message) { }
    }

    public class HardwareController {
        private readonly ArrayList sensorDataBuffer = new ArrayList();
        private readonly ArrayList sensors = new ArrayList();

        public HardwareController() { }

        public void AddSensor(Sensor sensor) {
            if (sensor != null) {
                sensors.Add(sensor);
            }
        }

        public Sensor GetSensor(int index) {
            if (index <= sensors.Count - 1) {
                if (sensors[index] is Sensor) {
                    return (Sensor)sensors[index];
                } else {
                    throw new ArgumentNullException();
                }
            } else {
                throw new IndexOutOfRangeException();
            }
        }

        public void RemoveSensorByID(int id) {
            int sensornum = 0;
            for (int k = 0; k < sensors.Count; k++) {
                var sensor = sensors[sensornum];
                if (sensor is Sensor) {
                    if (id == ((Sensor)sensor).ID) {
                        sensors.Remove(sensor);
                        sensornum--;
                    }
                }
                sensornum++;
            }
        }

        public void RemoveSensor(int index) {
            if (index <= sensors.Count - 1) {
                sensors.RemoveAt(index);
            } else {
                throw new IndexOutOfRangeException();
            }
        }

        public int GetSensorCount() { return sensors.Count; }

        public int GetSensorCount(Type stype) {
            int count = 0;
            foreach (object sensor in sensors) {
                if (sensor is Sensor) {
                    if (sensor.GetType() == stype) {
                        count++;
                    }
                }
            }
            return count;
        }

        public ArrayList GetAllSensors(Type stype) {
            ArrayList matches = new ArrayList();
            foreach (object sensor in sensors) {
                if (sensor is Sensor) {
                    if (sensor.GetType() == stype) {
                        matches.Add(sensor);
                    }
                }
            }
            return matches;
        }

        public void AddSensorReading(SensorData data) {
            if (data != null) {
                sensorDataBuffer.Add(data);
            }
        }

        public SensorData GetSensorReading(int index) {
            if (index <= sensorDataBuffer.Count - 1) {
                if (sensorDataBuffer[index] is SensorData) {
                    return (SensorData)sensorDataBuffer[index];
                } else {
                    throw new ArgumentNullException();
                }
            } else {
                throw new IndexOutOfRangeException();
            }
        }

        public SensorData PollSensorReading(int index) {
            if (index <= sensorDataBuffer.Count - 1) {
                if (sensorDataBuffer[index] is SensorData) {
                    var data = (SensorData)sensorDataBuffer[index];
                    sensorDataBuffer.RemoveAt(index);
                    return data;
                } else {
                    throw new ArgumentNullException();
                }
            } else {
                throw new IndexOutOfRangeException();
            }
        }

        public void RemoveSensorReading(int index) {
            if (index <= sensorDataBuffer.Count - 1) {
                sensorDataBuffer.RemoveAt(index);
            } else {
                throw new IndexOutOfRangeException();
            }
        }

        public int GetSensorDataBufferCount() { return sensorDataBuffer.Count; }
    }
}
using System;
using System.Collections;
using OccupOS.CommonLibrary.Sensors;
using System.Reflection;
using System.Threading;

namespace OccupOS.CommonLibrary.NodeControllers {

    public class StorageDeviceMissingException : Exception {
        public StorageDeviceMissingException(string message)
            : base(message) { }
    }

    public abstract class NodeController {
        private readonly ArrayList sensorDataBuffer = new ArrayList();
        private readonly ArrayList sensors = new ArrayList();
        private DynamicSensorController ds_controller = null;
        private Thread ds_thread = null;

        public void AddSensor(Sensor sensor) {
            if (sensor != null) {
                sensors.Add(sensor);
            }
        }

        private void AddSensor(Type stype, int count) {
            for (int k = 0; k < count; k++) {
                ConstructorInfo constructor = stype.GetConstructor(new Type[] { typeof(int) });
                if (constructor != null) {
                    Sensor newsensor = constructor.Invoke(new Object[] {
                        FindLowestNumID(GetAllSensors(stype), stype, 0) }) as Sensor;
                    AddSensor(newsensor);
                    if (newsensor is IDynamicSensor)
                        ((IDynamicSensor) newsensor).Connect();
                }
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
            }
            else {
                throw new IndexOutOfRangeException();
            }
        }

        private void RemoveInactiveSensors(Type stype) {
            ArrayList actives = GetAllSensors(stype);
            foreach (Sensor current_active in actives) {
                try {
                    if (current_active is IDynamicSensor) {
                        if (((IDynamicSensor)current_active).GetConnectionStatus()
                            == ConnectionStatus.Disconnected)
                            DisconnectSensor(current_active);
                    }
                } catch (SensorNotFoundException) {
                    DisconnectSensor(current_active);
                }
            }
        }

        private void DisconnectSensor(Sensor sensor) {
            if (sensor is IDynamicSensor)
                ((IDynamicSensor)sensor).Disconnect();
            RemoveSensorByID(sensor.ID);
        }

        private int FindLowestNumID(ArrayList sensorlist, Type stype, int startID) { //todo: replace with check database
            if (sensorlist.Count > 0) {
                foreach (Sensor current_active in sensorlist) {
                    if (startID.ToString().Equals(current_active.ID))
                        FindLowestNumID(sensorlist, stype, startID + 1);
                }
            }
            return startID;
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

        public void StartListening() {
            if (ds_controller == null) {
                ds_controller = new DynamicSensorController(this);
            }
            if (ds_thread == null) {
                ds_thread = new Thread(ds_controller.UpdateCycle);
                ds_thread.Priority = ThreadPriority.BelowNormal;
                ds_thread.Start();
            }
            ds_controller.Enable();
            //ds_thread.Resume();
        }

        public void StopListening() {
            if (ds_controller != null) {
                ds_controller.Disable();
            }
        }

        public void UpdateSensors() {
            foreach (var type in Assembly.GetAssembly(this.GetType()).GetTypes()) {
                if (type.IsClass) {
                    foreach (var iface in type.GetInterfaces()) {
                        if (iface.Name.Equals("IDynamicSensor")) {
                            ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(int) });
                            if (constructor != null) {
                                IDynamicSensor dsensor = constructor.Invoke(new Object[] { -1 }) as IDynamicSensor;
                                int sensors_connected = dsensor.GetDeviceCount();
                                int sensors_store = GetSensorCount(type);
                                if (sensors_connected > sensors_store) {
                                    AddSensor(type, sensors_connected - sensors_store);
                                } else {
                                    if (sensors_connected < sensors_store)
                                        RemoveInactiveSensors(type);
                                }
                            }
                        }
                    }
                }
            }
        }

        private class DynamicSensorController {
            private NodeController master = null;
            private Boolean enabled = false;

            public DynamicSensorController(NodeController master) {
                    this.master = master;
                }

            public void UpdateCycle() {
                while (true) {
                    if (!enabled) {
                        Thread.CurrentThread.Suspend();
                    } 
                    else {
                        master.UpdateSensors();
                    }
                }
            }

            public void Enable() {
                this.enabled = true;
            }

            public void Disable() {
                this.enabled = false;
            }
        }
    }
}
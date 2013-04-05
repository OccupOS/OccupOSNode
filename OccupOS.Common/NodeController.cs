using System;
using System.Collections;
using OccupOS.CommonLibrary.Sensors;
using OccupOS.CommonLibrary.HardwareControllers;
using OccupOS.CommonLibrary.NetworkControllers;
using System.Reflection;
using System.Threading;

namespace OccupOS.CommonLibrary.NodeControllers {

    public abstract class NodeController {
        private HardwareController hardware_controller = null;
        private NetworkController network_controller = null;
        private DynamicSensorController dyn_controller = null;
        private Thread ds_thread = null;

        public NodeController(HardwareController hardwareController, NetworkController networkController) {
            this.hardware_controller = hardwareController;
            this.network_controller = networkController;
        }

        private void AddSensor(Type stype, int count) {
            for (int k = 0; k < count; k++) {
                ConstructorInfo constructor = stype.GetConstructor(new Type[] { typeof(int) });
                if (constructor != null) {
                    Sensor newsensor = constructor.Invoke(new Object[] {
                        FindLowestNumID(hardware_controller.GetAllSensors(stype), stype, 0) }) as Sensor;
                    hardware_controller.AddSensor(newsensor);
                    if (newsensor is IDynamicSensor)
                        ((IDynamicSensor)newsensor).Connect();
                }
            }
        }

        private void RemoveInactiveSensors(Type stype) {
            ArrayList actives = hardware_controller.GetAllSensors(stype);
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
            hardware_controller.RemoveSensorByID(sensor.ID);
        }

        private int FindLowestNumID(ArrayList sensorlist, Type stype, int startID) {
            //This method preferably needs to check whole database instead
            if (sensorlist.Count > 0) {
                foreach (Sensor current_active in sensorlist) {
                    if (startID.ToString().Equals(current_active.ID))
                        FindLowestNumID(sensorlist, stype, startID + 1);
                }
            }
            return startID;
        }

        public void StartListening(ThreadPriority priority = ThreadPriority.Normal) {
            if (dyn_controller == null) {
                dyn_controller = new DynamicSensorController(this);
            }
            if (ds_thread == null) {
                ds_thread = new Thread(dyn_controller.UpdateCycle);
                ds_thread.Priority = priority;
                ds_thread.Start();
            }
            dyn_controller.Enable();
        }

        public void StopListening() {
            if (dyn_controller != null) {
                dyn_controller.Disable();
            }
        }

        public void UpdateDynamicSensors() {
            foreach (var type in Assembly.GetAssembly(this.GetType()).GetTypes()) {
                if (type.IsClass) {
                    foreach (var iface in type.GetInterfaces()) {
                        if (iface.Name.Equals("IDynamicSensor")) {
                            ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(int) });
                            if (constructor != null) {
                                IDynamicSensor dsensor = constructor.Invoke(new Object[] { -1 }) as IDynamicSensor;
                                int sensors_store = hardware_controller.GetSensorCount(type);
                                int max_sensors = dsensor.GetMaxSensors();
                                if (sensors_store < max_sensors || max_sensors < 0) {
                                    int sensors_connected = dsensor.GetDeviceCount();
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
        }

        private class DynamicSensorController {
            private NodeController master = null;
            private Boolean enabled = false;
            private ManualResetEvent event_waiter = new ManualResetEvent(false);

            public DynamicSensorController(NodeController master) {
                    this.master = master;
                }

            public void UpdateCycle() {
                while (true) {
                    if (!enabled) {
                        event_waiter.WaitOne();
                    } 
                    else {
                        master.UpdateDynamicSensors();
                    }
                }
            }

            public void Enable() {
                this.enabled = true;
                event_waiter.Set();
            }

            public void Disable() {
                this.enabled = false;
            }
        }
    }
}
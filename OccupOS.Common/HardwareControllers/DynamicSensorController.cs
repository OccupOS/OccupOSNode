namespace OccupOS.CommonLibrary.HardwareControllers {

    using System;
    using System.Collections;
    using System.Reflection;
    using System.Threading;
    using OccupOS.CommonLibrary.Sensors;

    public class DynamicSensorController {
        private HardwareController hw_controller = null;
        private Boolean enabled = false;
        private ManualResetEvent event_waiter = new ManualResetEvent(false);

        public DynamicSensorController(HardwareController hardwareController) {
            this.hw_controller = hardwareController;
        }

        public void Run() {
            while (true) {
                if (!enabled) {
                    event_waiter.WaitOne();
                } else {
                    UpdateDynamicSensors();
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

        private void UpdateDynamicSensors() {
            foreach (var type in Assembly.GetAssembly(this.GetType()).GetTypes()) {
                if (type.IsClass) {
                    foreach (var iface in type.GetInterfaces()) {
                        if (iface.Name.Equals("IDynamicSensor")) {
                            ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(int) });
                            if (constructor != null) {
                                IDynamicSensor dsensor = constructor.Invoke(new Object[] { -1 }) as IDynamicSensor;
                                int sensors_store = hw_controller.GetSensorCount(type);
                                int max_sensors = dsensor.GetMaxSensors();
                                if (sensors_store < max_sensors || max_sensors < 0) {
                                    int sensors_connected = dsensor.GetDeviceCount();
                                    if (sensors_connected > sensors_store) {
                                        CreateSensor(type, sensors_connected - sensors_store);
                                    } else {
                                        if (sensors_connected < sensors_store)
                                            DisposeInactives(type);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CreateSensor(Type stype, int count) {
            for (int k = 0; k < count; k++) {
                ConstructorInfo constructor = stype.GetConstructor(new Type[] { typeof(int) });
                if (constructor != null) {
                    Sensor newsensor = constructor.Invoke(new Object[] {
                        FindLowestNumID(hw_controller.GetAllSensors(stype), stype, 0) }) as Sensor;
                    hw_controller.AddSensor(newsensor);
                    if (newsensor is IDynamicSensor)
                        ((IDynamicSensor)newsensor).Connect();
                }
            }
        }

        private void DisposeSensor(Sensor sensor) {
            if (sensor is IDynamicSensor)
                ((IDynamicSensor)sensor).Disconnect();
            hw_controller.RemoveSensorByID(sensor.ID);
        }

        private void DisposeInactives(Type stype) {
            ArrayList actives = hw_controller.GetAllSensors(stype);
            foreach (Sensor current_active in actives) {
                try {
                    if (current_active is IDynamicSensor) {
                        if (((IDynamicSensor)current_active).GetConnectionStatus()
                            == ConnectionStatus.Disconnected)
                            DisposeSensor(current_active);
                    }
                } catch (SensorNotFoundException) {
                    DisposeSensor(current_active);
                }
            }
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
    }
}

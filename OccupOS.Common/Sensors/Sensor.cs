using System;

namespace OccupOS.CommonLibrary.Sensors {
    public abstract class Sensor {
        protected Sensor(String id) { ID = id; }
        public string ID { get; private set; }

        public abstract SensorData GetData();
    }
}
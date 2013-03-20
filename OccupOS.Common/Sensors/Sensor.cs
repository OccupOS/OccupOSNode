using System;

namespace OccupOS.CommonLibrary.Sensors {
    public abstract class Sensor {
        protected Sensor(int id) { ID = id; }
        public int ID { get; private set; }

        public abstract SensorData GetData();
    }
}
using System;

namespace OccupOS.CommonLibrary.Sensors {
    public abstract class Sensor {
        protected Sensor(String id) { ID = id; }
        public string ID { get; private set; }

        public abstract SensorData GetData();

        public virtual ConnectionStatus GetConnectionStatus() {
            return ConnectionStatus.Inapplicable;
        }

        public virtual void Connect() {
            throw new NotImplementedException();
        }

        public virtual void Disconnect() {
            throw new NotImplementedException();
        }
    }
}
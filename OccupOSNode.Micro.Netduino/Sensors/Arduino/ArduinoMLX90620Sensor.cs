using System;
using OccupOS.CommonLibrary.Sensors;

namespace OccupOSNode.Micro.Sensors.Arduino {
    internal class ArduinoMLX90620Sensor : Sensor, IEntityCountSensor {
        public ArduinoMLX90620Sensor(string id)
            : base(id) { }

        public int GetEntityCount() { throw new NotImplementedException(); }
        public override SensorData GetData() { throw new NotImplementedException(); }
    }
}
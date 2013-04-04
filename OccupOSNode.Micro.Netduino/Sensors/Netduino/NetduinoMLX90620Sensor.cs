using System;
using OccupOS.CommonLibrary.Sensors;

namespace OccupOSNode.Micro.Sensors.Netduino {
    internal class NetduinoMLX90620Sensor : Sensor, IEntityCountSensor {
        public NetduinoMLX90620Sensor(int id)
            : base(id) { }

        public int GetEntityCount() { throw new NotImplementedException(); }
        public override SensorData GetData() { throw new NotImplementedException(); }
    }
}
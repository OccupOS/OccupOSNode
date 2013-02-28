using System;

namespace OccupOS.CommonLibrary.Sensors {
    //[Serializable]
    public class SensorNotFoundException : Exception {
        public SensorNotFoundException() { }

        public SensorNotFoundException(String msg)
            : base(msg) { }

        public SensorNotFoundException(string message, Exception inner)
            : base(message, inner) { }

        /* No System.Runtime.Serialization for micro framework?
        protected SensorNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {} */
    }
}
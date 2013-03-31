namespace OccupOS.CommonLibrary.Sensors {

    using System;

    public struct Position {
        public float Depth;
        public int X;
        public int Y;
    }

    public class SensorData {
        public DateTime ReadTime;
        public DateTime PollTime;
        public Sensor Sensorobj;
        public float AnalogLight;
        public int EntityCount;
        public Position[] EntityPositions;
        public float Humidity;
        public float Pressure;
        public float Temperature;
    }

    public enum ConnectionStatus {
        Connected,
        Disconnected,
        Connecting,
        Disconnecting,
        Error,
        Unknown,
        Inapplicable
    }
}
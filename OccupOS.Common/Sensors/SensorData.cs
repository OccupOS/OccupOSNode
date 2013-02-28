namespace OccupOS.CommonLibrary.Sensors {
    public struct Position {
        public float Depth;
        public int X;
        public int Y;
    }

    public class SensorData {
        public float AnalogLight;

        public int EntityCount;
        public Position[] EntityPositions;
        public float Humidity;
        public float Pressure;
        public float Temperature;
    }
}
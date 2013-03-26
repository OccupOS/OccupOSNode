// TODO: Note that this is NOT supposed to reside in the OccupOSNode.Micro.Gadgeteer project. 
// It is a temporary hack until Gadgeteer supports the .NET MF 4.3 Framework.

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
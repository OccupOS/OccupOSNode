namespace OccupOS.CommonLibrary {
    using OccupOS.CommonLibrary.Sensors;

    public class PacketFactory
    {
        public string CreatePacket(SensorData sensorData)
        {
            string packet = "";
            packet = packet.AddSeperatedValue(System.DateTime.Now.ToString(),",");
            packet = packet.AddSeperatedValue(sensorData.AnalogLight.ToString(),",");
            packet = packet.AddSeperatedValue(sensorData.EntityCount.ToString(),",");
            if (sensorData.EntityPositions != null) {
                foreach (Position position in sensorData.EntityPositions) {
                    packet = packet.AddSeperatedValue(position.X.ToString(), ",");
                    packet = packet.AddSeperatedValue(position.Y.ToString(), ",");
                    packet = packet.AddSeperatedValue(position.Depth.ToString(), ",");
                }
            }
            packet = packet.AddSeperatedValue(sensorData.Humidity.ToString(),",");
            packet = packet.AddSeperatedValue(sensorData.Pressure.ToString(),",");
            packet = packet.AddSeperatedValue(sensorData.Temperature.ToString(),",");
            return packet;
        }
    }

    public static class StringExtension {
        public static string AddSeperatedValue(this string str, string value, string symbol)
        {
            return str.Length == 0 ? value : str + symbol + value;
        }
    }
}
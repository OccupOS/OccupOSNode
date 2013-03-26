namespace OccupOS.CommonLibrary {
    using OccupOS.CommonLibrary.Sensors;

    public static class PacketFactory
    {
        public static string CreatePacket(SensorData sensorData)
        {
            string packet = "";
            packet = packet.AddCommaSeperatedValue(System.DateTime.Now.ToString());
            packet = packet.AddCommaSeperatedValue(sensorData.AnalogLight.ToString());
            return packet;
        }
    }

    public static class StringExtension {
        public static string AddCommaSeperatedValue(this string str, string value)
        {
            return str.Length == 0 ? value : str + "," + value;
        }
    }
}
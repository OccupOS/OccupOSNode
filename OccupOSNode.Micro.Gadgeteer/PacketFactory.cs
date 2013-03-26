// TODO: Note that this is NOT supposed to reside in the OccupOSNode.Micro.Gadgeteer project. 
// It is a temporary hack until Gadgeteer supports the .NET MF 4.3 Framework.

namespace GadgeteerDemo
{
    internal class PacketFactory
    {
        public static string CreatePacket(SensorData sensorData)
        {
            string packet = "";
            packet = packet.AddCommaSeperatedValue(System.DateTime.Now.ToString());
            packet = packet.AddCommaSeperatedValue(sensorData.AnalogLight.ToString());
            return packet;
        }
    }

    internal static class StringExtension
    {
        public static string AddCommaSeperatedValue(this string str, string value)
        {
            return str.Length == 0 ? value : str + "," + value;
        }
    }
}
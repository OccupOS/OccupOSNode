// TODO: Note that this is NOT supposed to reside in the OccupOSNode.Micro.Gadgeteer project. 
// It is a temporary hack until Gadgeteer supports the .NET MF 4.3 Framework.

namespace GadgeteerDemo {
    using System;

    public class PacketFactory {
        public static string CreatePacket(SensorData sensorData) {
            string packet = "";
            packet = packet.AddSeperatedValue(System.DateTime.Now.ToString(), ",");
            packet = packet.AddSeperatedValue(sensorData.AnalogLight.ToString(), ",");
            packet = packet.AddSeperatedValue(sensorData.EntityCount.ToString(), ",");
            packet = packet.AddSeperatedValue(sensorData.Humidity.ToString(), ",");
            packet = packet.AddSeperatedValue(sensorData.Pressure.ToString(), ",");
            packet = packet.AddSeperatedValue(sensorData.Temperature.ToString(), ",");
            return packet;
        }
    }

    public static class StringExtension {
        public static string AddSeperatedValue(this string str, string value, string symbol) {
            return str.Length == 0 ? value : str + symbol + value;
        }
    }
}
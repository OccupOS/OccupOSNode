namespace OccupOS.CommonLibrary {
    using OccupOS.CommonLibrary.Sensors;
    using System;

    public class PacketFactory {
        public static string CreatePacket(SensorData sensorData) {
            string packet = "";
            packet = packet.AddSeperatedValue(System.DateTime.Now.ToString(), ",");
            packet = packet.AddSeperatedValue(sensorData.AnalogLight.ToString(), ",");
            packet = packet.AddSeperatedValue(sensorData.EntityCount.ToString(), ",");
            if (sensorData.EntityPositions != null) {
                foreach (Position position in sensorData.EntityPositions) {
                    packet = packet.AddSeperatedValue(position.X.ToString(), ",");
                    packet = packet.AddSeperatedValue(position.Y.ToString(), ",");
                    packet = packet.AddSeperatedValue(position.Depth.ToString(), ",");
                }
            }
            packet = packet.AddSeperatedValue(sensorData.Humidity.ToString(), ",");
            packet = packet.AddSeperatedValue(sensorData.Pressure.ToString(), ",");
            packet = packet.AddSeperatedValue(sensorData.Temperature.ToString(), ",");
            return packet;
        }

        public static string SerializeJSON(int nodeID, SensorData[] sensorData) {
            string jsonstring = "{";
            if (nodeID >= 0) {
                jsonstring = jsonstring + "\"" + nodeID + "\":{";
                if (sensorData != null) {
                    foreach (SensorData dataobj in sensorData) {
                        jsonstring = jsonstring + "\"" + dataobj.Sensorobj.ID.ToString() + "\":{";
                        jsonstring = jsonstring + SerializeDataComponent(dataobj) + "}";
                    }
                }
                jsonstring = jsonstring + "}";
            }
            return jsonstring + "}";
        }

        private static string SerializeDataComponent(SensorData dataobj) {
            string jsonfragment = "";
                Type sensortype = dataobj.Sensorobj.GetType();
                if (sensortype.IsClass) {
                    int k = 0;
                    string[] artefacts = new string[sensortype.GetInterfaces().Length];
                    foreach (Type iface in sensortype.GetInterfaces()) {
                        artefacts[k] = SerializeInterface(iface.Name, dataobj);
                        k++;
                    }
                    for (int l = 0; l < artefacts.Length; l++) {
                        if (l < artefacts.Length - 1) {
                            if (jsonfragment != "" && artefacts[l] != "")
                                jsonfragment = jsonfragment + ",";
                        }
                        jsonfragment = jsonfragment + artefacts[l];
                    }
            }
            return jsonfragment;
        }

        private static string SerializeInterface(string ifaceName, SensorData dataobj) {
            string jsonfragment = "";
            switch (ifaceName) {
                case "IEntityCountSensor":
                    jsonfragment = jsonfragment + "\"0\":" + dataobj.EntityCount;
                    break;
                case "IEntityPositionSensor": 
                    //todo
                    break;
                case "IHumiditySensor":
                    jsonfragment = jsonfragment + "\"5\":" + dataobj.Humidity;
                    break;
                case "ILightSensor": break;
                case "IPowerSensor": break;
                case "IPressureSensor": 
                    jsonfragment = jsonfragment + "\"7\":" + dataobj.Pressure;
                    break;
                case "ISound": break;
                case "ITemperatureSensor": 
                    jsonfragment = jsonfragment + "\"9\":" + dataobj.Temperature; 
                    break;
                case "IVibrationSensor": break;
                case "IWindSpeedSensor": break;
                default: break;
            }
            return jsonfragment;
        }
    }

    public static class StringExtension {
        public static string AddSeperatedValue(this string str, string value, string symbol)
        {
            return str.Length == 0 ? value : str + symbol + value;
        }
    }
}
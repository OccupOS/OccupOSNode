namespace OccupOS.CommonLibrary {
    using OccupOS.CommonLibrary.Sensors;
    using System;

    public class PacketFactory {

        //JSON Identifiers:
        private static char READTIME_ID = 'a';
        private static char POLLTIME_ID = 'b';
        private static int ENTITYCOUNT_ID = 0;
        private static int ENTITYPOS_ID = 1;
        private static int XPOS_ID = 99; //todo
        private static int YPOS_ID = 99; //todo
        private static int DEPTHPOS_ID = 99; //todo
        private static int HUMIDITY_ID = 5;
        private static int AIRQUALITY_ID = 6;
        private static int LIGHT_ID = 3;
        private static int POWER_ID = 8;
        private static int PRESSURE_ID = 7;
        private static int SOUND_ID = 2;
        private static int TEMPERATURE_ID = 9;
        private static int VIBRATION_ID = 4;
        private static int WINDSPEED_ID = 10;
        private static int GASDETECT_ID = 11;

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
                if (dataobj.ReadTime != DateTime.MinValue)
                    jsonfragment = jsonfragment + "\"" + READTIME_ID + "\":"
                        + dataobj.ReadTime.ToString("dd'/'MM'/'yyyy hh':'mm':'ss'.'nn"); //todo: test
                if (dataobj.PollTime != DateTime.MinValue)
                    if (jsonfragment != "")
                        jsonfragment = jsonfragment + ",";
                    jsonfragment = jsonfragment + "\"" + POLLTIME_ID + "\":"
                        + dataobj.PollTime.ToString("dd'/'MM'/'yyyy hh':'mm':'ss'.'nn");
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
                    jsonfragment = jsonfragment + "\"" + ENTITYCOUNT_ID + "\":" + dataobj.EntityCount;
                    break;
                case "IEntityPositionSensor":
                    foreach (Position pos in dataobj.EntityPositions) {
                        jsonfragment = jsonfragment + "\"" + ENTITYPOS_ID + "\":{"; //todo
                        jsonfragment = jsonfragment + "}";
                    }
                    JsonCollection();
                    break;
                case "IHumiditySensor":
                    jsonfragment = jsonfragment + "\"" + HUMIDITY_ID + "\":" + dataobj.Humidity;
                    break;
                case "ILightSensor":
                    jsonfragment = jsonfragment + "\"" + LIGHT_ID + "\":" + dataobj.AnalogLight;
                    break;
                case "IPowerSensor": break;
                case "IPressureSensor":
                    jsonfragment = jsonfragment + "\"" + PRESSURE_ID + "\":" + dataobj.Pressure;
                    break;
                case "ISound": break;
                case "ITemperatureSensor":
                    jsonfragment = jsonfragment + "\"" + TEMPERATURE_ID + "\":" + dataobj.Temperature;
                    break;
                case "IVibrationSensor": break;
                case "IWindSpeedSensor": break;
                default: break;
            }
            return jsonfragment;
        }

        public static void JsonCollection() {
        //todo
        }
    }

    public static class StringExtension {
        public static string AddSeperatedValue(this string str, string value, string symbol)
        {
            return str.Length == 0 ? value : str + symbol + value;
        }
    }
}
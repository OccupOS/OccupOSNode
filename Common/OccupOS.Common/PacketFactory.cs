// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PacketFactory.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace OccupOS.CommonLibrary
{
    using System;

    using OccupOS.CommonLibrary.Sensors;

    public class PacketFactory
    {
        // JSON Identifiers:
        private const int AIRQUALITY_ID = 6;

        private const int DEPTHPOS_ID = 99; // todo

        private const int ENTITYCOUNT_ID = 0;

        private const int ENTITYPOS_ID = 1;

        private const int GASDETECT_ID = 11;

        private const int HUMIDITY_ID = 5;

        private const int LIGHT_ID = 3;

        private const char POLLTIME_ID = 'b';

        private const int POWER_ID = 8;

        private const int PRESSURE_ID = 7;

        private const char READTIME_ID = 'a';

        private const int SOUND_ID = 2;

        private const int TEMPERATURE_ID = 9;

        private const int VIBRATION_ID = 4;

        private const int WINDSPEED_ID = 10;

        private const int XPOS_ID = 99; // todo

        private const int YPOS_ID = 99; // todo

        public static string CreatePacket(SensorData sensorData)
        {
            string packet = string.Empty;
            packet = packet.AddSeperatedValue(System.DateTime.Now.ToString(), ",");
            packet = packet.AddSeperatedValue(sensorData.AnalogLight.ToString(), ",");
            packet = packet.AddSeperatedValue(sensorData.EntityCount.ToString(), ",");
            packet = packet.AddSeperatedValue(sensorData.Humidity.ToString(), ",");
            packet = packet.AddSeperatedValue(sensorData.Pressure.ToString(), ",");
            packet = packet.AddSeperatedValue(sensorData.Temperature.ToString(), ",");
            return packet;
        }

        public static void JsonCollection()
        {
            // todo
        }

        public static string SerializeJSON(int nodeID, SensorData[] sensorData)
        {
            string jsonstring = "{";
            if (nodeID >= 0)
            {
                jsonstring = jsonstring + "\"" + nodeID + "\":{";
                if (sensorData != null)
                {
                    foreach (SensorData dataobj in sensorData)
                    {
                        jsonstring = jsonstring + "\"" + dataobj.Sensorobj.ID + "\":{";
                        jsonstring = jsonstring + SerializeDataComponent(dataobj) + "}";
                        //needs commas
                    }
                }

                jsonstring = jsonstring + "}";
            }

            return jsonstring + "}";
        }

        private static string SerializeDataComponent(SensorData dataobj)
        {
            string jsonfragment = string.Empty;
            Type sensortype = dataobj.Sensorobj.GetType();
            if (sensortype.IsClass)
            {
                int k = 0;
                if (dataobj.ReadTime != DateTime.MinValue)
                {
                    jsonfragment = jsonfragment + "\"" + READTIME_ID + "\":"
                                                + dataobj.ReadTime.ToString("dd'/'MM'/'yyyy hh':'mm':'ss'.'nn");

                    // todo: test
                }

                if (dataobj.PollTime != DateTime.MinValue)
                {
                    if (jsonfragment != string.Empty)
                    {
                        jsonfragment = jsonfragment + ",";
                    }
                }

                jsonfragment = jsonfragment + "\"" + POLLTIME_ID + "\":"
                                            + dataobj.PollTime.ToString("dd'/'MM'/'yyyy hh':'mm':'ss'.'nn");
                string[] artefacts = new string[sensortype.GetInterfaces().Length];
                foreach (Type iface in sensortype.GetInterfaces())
                {
                    artefacts[k] = SerializeInterface(iface.Name, dataobj);
                    k++;
                }

                for (int l = 0; l < artefacts.Length; l++)
                {
                    if (l < artefacts.Length - 1)
                    {
                        if (jsonfragment != string.Empty && artefacts[l] != string.Empty)
                        {
                            jsonfragment = jsonfragment + ",";
                        }
                    }

                    jsonfragment = jsonfragment + artefacts[l];
                }
            }

            return jsonfragment;
        }

        private static string SerializeInterface(string ifaceName, SensorData dataobj)
        {
            string jsonfragment = string.Empty;
            switch (ifaceName)
            {
                case "IEntityCountSensor":
                    jsonfragment = jsonfragment + "\"" + ENTITYCOUNT_ID + "\":" + dataobj.EntityCount;
                    break;
                case "IEntityPositionSensor":
                    foreach (Position pos in dataobj.EntityPositions)
                    {
                        jsonfragment = jsonfragment + "\"" + ENTITYPOS_ID + "\":{"; // todo
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
                case "IPowerSensor":
                    break;
                case "IPressureSensor":
                    jsonfragment = jsonfragment + "\"" + PRESSURE_ID + "\":" + dataobj.Pressure;
                    break;
                case "ISound":
                    break;
                case "ITemperatureSensor":
                    jsonfragment = jsonfragment + "\"" + TEMPERATURE_ID + "\":" + dataobj.Temperature;
                    break;
                case "IVibrationSensor":
                    break;
                case "IWindSpeedSensor":
                    break;
                default:
                    break;
            }

            return jsonfragment;
        }
    }

    public static class StringExtension
    {
        public static string AddSeperatedValue(this string str, string value, string symbol)
        {
            return str.Length == 0 ? value : str + symbol + value;
        }
    }
}
using System;
using System.IO;
using OccupOS.CommonLibrary.NodeControllers;

namespace OccupOSNode.Micro {
    internal class ArduinoNodeController : NodeController {
        public ArduinoNodeController() {
            var rootDirectory = new DirectoryInfo(@"\SD\");
            if (rootDirectory.Exists) {
                LoadConfiguration();
            }
            else {
                throw new StorageDeviceMissingException("Couldn't find a connected SD card.");
            }
        }

        protected override void LoadConfiguration() { throw new NotImplementedException(); }

        public void PollSensors() {
            for (int k = 0; k < GetSensorCount(); k++) {
                AddSensorReading(GetSensor(k).GetData());
            }
        }
    }
}
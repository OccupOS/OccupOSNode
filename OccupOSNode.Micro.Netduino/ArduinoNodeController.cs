// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArduinoNodeController.cs" company="OccupOS">
//   This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License along with this program. If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The arduino node controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOSNode.Micro
{
    using System;
    using System.IO;
    using OccupOS.CommonLibrary.NodeControllers;
    using Microsoft.SPOT.Hardware;
    using SecretLabs.NETMF.Hardware.NetduinoPlus;

    internal class ArduinoNodeController : NodeController
    {
        private static OutputPort out_port = null;
        private static TristatePort tri_port = null;

        public ArduinoNodeController()
        {
            /*var rootDirectory = new DirectoryInfo(@"\SD\");
            if (rootDirectory.Exists)
            {
                this.LoadConfiguration();
            }
            else
            {
                throw new StorageDeviceMissingException("Couldn't find a connected SD card.");
            }*/
        }

        public void PollSensors()
        {
            for (int k = 0; k < this.GetSensorCount(); k++)
            {
                this.AddSensorReading(this.GetSensor(k).GetData());
            }
        }

        protected override void LoadConfiguration() //remove?
        {
            throw new NotImplementedException();
        }

        public static Boolean AttemptSetOutputPort(Cpu.Pin portID, bool initialState) {
            if (out_port == null) {
                out_port = new OutputPort(portID, initialState);
                return true;
            } else return false;
        }

        public static Boolean AttemptSetTristatePort(Cpu.Pin portID, bool initialState, bool glitchFilter, Port.ResistorMode resistor) {
            if (tri_port == null) {
                tri_port = new TristatePort(portID, initialState, glitchFilter, resistor);
                return true;
            } else return false;
        }

        public static OutputPort GetOutputPort() {
            return out_port;
        }

        public static TristatePort GetTristatePort() {
            return tri_port;
        }

        public static void DisposeOutputPort() {
            out_port.Dispose();
            out_port = null;
        }

        public static void DisposeTristatePort() {
            tri_port.Dispose();
            tri_port = null;
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetduinoHardwareController.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOSNode.Micro.HardwareControllers.Netduino
{
    using System;

    using Microsoft.SPOT;
    using Microsoft.SPOT.Hardware;

    using SecretLabs.NETMF.Hardware.NetduinoPlus;

    public class NetduinoHardwareController : OccupOS.CommonLibrary.HardwareControllers.HardwareController
    {
        private static OutputPort outPort = null;

        private static TristatePort triPort = null;

        public static bool AttemptSetOutputPort(Cpu.Pin portID, bool initialState)
        {
            if (outPort == null)
            {
                try
                {
                    outPort = new OutputPort(portID, initialState);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.Print("Setting output request failed:\n" + e);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool AttemptSetTristatePort(
            Cpu.Pin portID, bool initialState, bool glitchFilter, Port.ResistorMode resistor)
        {
            if (triPort == null)
            {
                try
                {
                    triPort = new TristatePort(portID, initialState, glitchFilter, resistor);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.Print("Setting triport request failed:\n" + e);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static void BlinkLED(int delay, int count)
        {
            if (AttemptSetOutputPort(Pins.ONBOARD_LED, false))
            {
                for (int k = 0; k < count; k++)
                {
                    System.Threading.Thread.Sleep(delay);
                    outPort.Write(true);
                    System.Threading.Thread.Sleep(delay);
                    outPort.Write(false);
                }
            }
        }

        public static void DisposeOutputPort()
        {
            if (outPort != null)
            {
                outPort.Dispose();
            }

            outPort = null;
        }

        public static void DisposeTristatePort()
        {
            if (triPort != null)
            {
                triPort.Dispose();
            }

            triPort = null;
        }

        public static OutputPort GetOutputPort()
        {
            return outPort;
        }

        public static TristatePort GetTristatePort()
        {
            return triPort;
        }
    }
}
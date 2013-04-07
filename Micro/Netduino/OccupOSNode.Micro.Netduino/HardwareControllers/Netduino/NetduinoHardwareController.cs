namespace OccupOSNode.Micro.HardwareControllers.Netduino {

    using System;
    using Microsoft.SPOT;
    using Microsoft.SPOT.Hardware;
    using SecretLabs.NETMF.Hardware.NetduinoPlus;

    public class NetduinoHardwareController : OccupOS.CommonLibrary.HardwareControllers.HardwareController {

        private static OutputPort out_port = null;
        private static TristatePort tri_port = null;

        public static Boolean AttemptSetOutputPort(Cpu.Pin portID, bool initialState) {
            if (out_port == null) {
                try {
                    out_port = new OutputPort(portID, initialState);
                    return true;
                } catch (Exception e) { 
                    Debug.Print("Setting output request failed:\n" + e);
                    return false;
                }
            } else return false;
        }

        public static Boolean AttemptSetTristatePort(Cpu.Pin portID, bool initialState, bool glitchFilter, Port.ResistorMode resistor) {
            if (tri_port == null) {
                try {
                    tri_port = new TristatePort(portID, initialState, glitchFilter, resistor);
                    return true;
                } catch (Exception e) {
                    Debug.Print("Setting triport request failed:\n" + e);
                    return false;
                }
            } else return false;
        }

        public static OutputPort GetOutputPort() {
            return out_port;
        }

        public static TristatePort GetTristatePort() {
            return tri_port;
        }

        public static void DisposeOutputPort() {
            if (out_port != null)
                out_port.Dispose();
            out_port = null;
        }

        public static void DisposeTristatePort() {
            if (tri_port != null)
                tri_port.Dispose();
            tri_port = null;
        }

        public static void BlinkLED(int delay, int count) {
            if (AttemptSetOutputPort(Pins.ONBOARD_LED, false)) {
                for (int k = 0; k < count; k++) {
                    System.Threading.Thread.Sleep(delay);
                    out_port.Write(true);
                    System.Threading.Thread.Sleep(delay);
                    out_port.Write(false);
                }
            }
        }
    }
}
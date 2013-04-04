using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Toolbox.NETMF.Hardware;
using Toolbox.NETMF.NET;
using Toolbox.NETMF;

namespace OccupOSNode.Micro.NetworkControllers.Arduino {

    public class NetduinoWirelessNetworkController : OccupOS.CommonLibrary.NetworkControllers.NetworkController {

        private WiFlyGSX wf_module;
        private SimpleSocket socket = null;

        public NetduinoWirelessNetworkController() {
            wf_module = new WiFlyGSX();
        }

        public bool sendCommand(String command) {
            if (socket != null && command != null) {
                try {
                    Byte[] cmdBytes = Encoding.UTF8.GetBytes((command + "\r\n"));
                    socket.SendBinary(cmdBytes);
                    return true;
                } catch (InvalidOperationException e) {
                    Debug.Print("Failure to write to target:\n" + e);
                    socket = null;
                    return false;
                }
            }
            return false;
        }

        public void close(String closecommand = "QUIT") {
            if (socket != null) {
                sendCommand(closecommand);
            }
            wf_module.CloseSocket();
            socket.Close();
            socket = null;
        }

        public void ConnectToHost(String SSID, String key, string hostname, ushort hostport) {
            wf_module.EnableDHCP();
            wf_module.JoinNetwork(SSID, 0, WiFlyGSX.AuthMode.MixedWPA1_WPA2, key);
            socket = new WiFlySocket(hostname, hostport, wf_module);
            socket.Connect();
        }
    }
}

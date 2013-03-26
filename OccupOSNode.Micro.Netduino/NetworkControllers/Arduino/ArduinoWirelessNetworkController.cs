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

    public class ArduinoWirelessNetworkController : NetworkController {

        private WiFlyGSX wf_module;
        private SimpleSocket socket = null;

        public ArduinoWirelessNetworkController() {
            wf_module = new WiFlyGSX();
        }

        public void sendCommand(String command) {
            if (socket != null && command != null) {
                Byte[] cmdBytes = Encoding.UTF8.GetBytes((command + "\r\n"));
                socket.SendBinary(cmdBytes); //can't write while disconnected!
            }
        }

        public void close() {
            if (socket != null) {
                sendCommand("QUIT");
            }
        }

        public void ConnectToNetworkHost(String SSID, String key, string hostname, ushort hostport) {
            wf_module.EnableDHCP();
            wf_module.JoinNetwork(SSID, 0, WiFlyGSX.AuthMode.MixedWPA1_WPA2, key);
            socket = new WiFlySocket(hostname, hostport, wf_module);
            socket.Connect();
        }
    }
}

using System;
using System.Net.Sockets;
using System.Text;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Toolbox.NETMF.Hardware;
using Toolbox.NETMF.NET;

namespace OccupOSNode.Micro.NetworkControllers.Netduino {

    public class NetduinoWirelessNetworkController : OccupOS.CommonLibrary.NetworkControllers.NetworkController {

        private WiFlyGSX wf_module;
        private SimpleSocket socket = null;

        public NetduinoWirelessNetworkController(string hostname, ushort port) 
        : base(hostname, port) {
            wf_module = new WiFlyGSX();
        }

        public override int SendData(string data) {
            if (socket != null && data != null) {
                try {
                    Byte[] cmdBytes = Encoding.UTF8.GetBytes((data + "\r\n"));
                    socket.SendBinary(cmdBytes);
                    return cmdBytes.Length;
                } catch (InvalidOperationException e) {
                    Debug.Print("Failure to write to target:\n" + e);
                    socket = null;
                    return 0;
                }
            }
            return 0;
        }

        public override void Disconnect() {
            wf_module.CloseSocket();
            socket.Close();
            socket = null;
        }

        public override void Connect(string SSID, string key) {
            wf_module.EnableDHCP();
            wf_module.JoinNetwork(SSID, 0, WiFlyGSX.AuthMode.MixedWPA1_WPA2, key);
            socket = new WiFlySocket(hostname, port, wf_module);
            socket.Connect();
        }
    }
}

using System;
using System.Text;
using Microsoft.SPOT;
using Toolbox.NETMF;
using Toolbox.NETMF.Hardware;
using Toolbox.NETMF.NET;

namespace OccupOSNode.Micro.NetworkControllers.Arduino {
    public class ArduinoWirelessNetworkController : NetworkController {
        private String address;
        private SimpleSocket socket;

        public ArduinoWirelessNetworkController(String hostName, int port) {

            WiFlyGSX WifiModule = new WiFlyGSX();
            WifiModule.EnableDHCP();
            WifiModule.JoinNetwork(hostName);

            socket = new WiFlySocket(hostName, 80, WifiModule);
            socket.Connect();

        }

        public void sendData(string data) {

            //byte[] buffer = Encoding.UTF8.GetBytes(data);
            socket.Send(data);
        }

        public void disconnect() {
            socket.Close();
        }

    }
}

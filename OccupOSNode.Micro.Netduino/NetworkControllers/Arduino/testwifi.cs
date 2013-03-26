using System;
using System.Text;
using Microsoft.SPOT;
using Toolbox.NETMF;
using Toolbox.NETMF.Hardware;
using Toolbox.NETMF.NET;

namespace OccupOSNode.Micro.NetworkControllers.Arduino {
    public class testWifi : NetworkController {

        private String address;
        private static SimpleSocket socket;

        public testWifi(String hostName, int port) {

            WiFlyGSX WifiModule = new WiFlyGSX();
            Debug.Print("connection started");
            WifiModule.EnableDHCP();
            WifiModule.JoinNetwork("testhoc", 0, WiFlyGSX.AuthMode.MixedWPA1_WPA2, "1234567890");
            Debug.Print("connection made");
            SimpleSocket socket = new WiFlySocket(hostName, 1333, WifiModule);
            socket.Connect();
            Debug.Print("connection established");

        }

        public static void sendData(String data) {

            //byte[] buffer = Encoding.UTF8.GetBytes(data);
            //return socket.Send(buffer);


            Byte[] cmdBytes = Encoding.UTF8.GetBytes((data + "\r\n"));
            socket.SendBinary(cmdBytes);

        }

        public void disconnect() {
            socket.Close();
        }

    }
}
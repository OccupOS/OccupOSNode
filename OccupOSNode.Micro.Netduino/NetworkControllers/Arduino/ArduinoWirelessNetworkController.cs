using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Toolbox.NETMF.Hardware;
using Toolbox.NETMF;

namespace OccupOSNode.Micro.NetworkControllers.Arduino {
    using System.Text;

    using Toolbox.NETMF.NET;

    public class ArduinoWirelessNetworkController : NetworkController
    {
        static WiFlyGSX wifly;
        static string SSID = "MorrisonN4";
        static string IP = "10.111.127.254";
        static string user = "YesItDoes";
        static string password = "GadgeteerSucks";
        private static SimpleSocket clientSocket;

        public ArduinoWirelessNetworkController()
        {
            
            connectToWifi();
            Thread.Sleep(10000);
            clientSocket = new WiFlySocket(IP, 49161, wifly);

            try
            {
                SimpleSocket.SocketProtocol Protocol = SimpleSocket.SocketProtocol.TcpStream;
                clientSocket.Connect(Protocol);
            }
            catch
            {
                throw new IOException("Can't connect");
            }

            sendCommand("hello world");
            close();
            clientSocket.Close();
        }

        public void sendCommand(String command)
        {
            Byte[] cmdBytes = Encoding.UTF8.GetBytes((command + "\r\n"));
            clientSocket.SendBinary(cmdBytes);
        }

        public void close() 
        {
            if (clientSocket != null) 
            {
                sendCommand("QUIT");
            }
        }

        public static void connectToWifi() 
        {
            wifly = new WiFlyGSX();
            wifly.EnableDHCP();
            wifly.JoinNetwork(SSID, 0, WiFlyGSX.AuthMode.MixedWPA1_WPA2, password);
            Thread.Sleep(3500);

            // Showing some interesting output
            Debug.Print("Local IP: " + wifly.LocalIP);
            Debug.Print("MAC address: " + wifly.MacAddress);

        }
    }
}

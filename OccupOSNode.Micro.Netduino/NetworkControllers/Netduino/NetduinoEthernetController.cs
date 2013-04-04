namespace OccupOSNode.Micro.NetworkControllers.Netduino {

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

    public class NetduinoEthernetController : OccupOS.CommonLibrary.NetworkControllers.NetworkController {
        private readonly IPAddress hostAddress;
        private readonly IPEndPoint remoteEndPoint;
        private string address;
        private IPHostEntry hostEntry;
        private Socket socket;

        public NetduinoEthernetController(string hostName, ushort port) 
            : base(hostName, port) {
            address = hostName;
            hostEntry = Dns.GetHostEntry(hostName);
            hostAddress = hostEntry.AddressList[0];
            hostAddress = IPAddress.Parse(hostName);
            remoteEndPoint = new IPEndPoint(hostAddress, port);
        }

        public override Boolean Connect(string SSID, string key) {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                socket.Connect(remoteEndPoint);
            } catch (SocketException e) {
                return false;
            }
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            socket.SendTimeout = 5000;
            return true;
        }

        public override void Disconnect() {
            socket.Close();
            socket = null;
        }

        public override int SendData(string data) {
            if (socket != null) {
                try {
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    return socket.Send(buffer);
                } catch (Exception e) {
                    if (e is SocketException || e is ObjectDisposedException) {
                        socket = null;
                    }
                    return 0;
                }
            }
            return 0;
        }
    }
}
namespace OccupOSNode.NetworkControllers {

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using OccupOS.CommonLibrary.NetworkControllers;

    class FullEthernetController : OccupOS.CommonLibrary.NetworkControllers.NetworkController {
        private Socket socket;

        public FullEthernetController(string hostname, ushort port) 
            : base(hostname, port) {
        }

        public override Boolean Connect(string SSID, string key) {
            IPHostEntry hostEntry = Dns.GetHostEntry(hostname);
            IPAddress hostAddress = hostEntry.AddressList[0];
            try {
                hostAddress = IPAddress.Parse(hostname);
            } catch (Exception e) {
                if (e is ArgumentException || e is FormatException)
                    hostAddress = Dns.GetHostAddresses(hostname)[0];
            }
            IPEndPoint remoteEndPoint = new IPEndPoint(hostAddress, port);
            socket = new Socket(hostAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
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

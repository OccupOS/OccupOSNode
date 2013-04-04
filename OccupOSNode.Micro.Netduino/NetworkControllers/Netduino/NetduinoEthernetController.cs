using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OccupOSNode.Micro.NetworkControllers.Arduino {
    public class NetduinoEthernetController : OccupOS.CommonLibrary.NetworkControllers.NetworkController {
        private readonly IPAddress hostAddress;
        private readonly IPEndPoint remoteEndPoint;
        private string address;
        private IPHostEntry hostEntry;
        private Socket socket;

        public NetduinoEthernetController(string hostName, int port) {
            address = hostName;
            //hostEntry = Dns.GetHostEntry(hostName);
            //hostAddress = hostEntry.AddressList[0];
            hostAddress = IPAddress.Parse(hostName);
            remoteEndPoint = new IPEndPoint(hostAddress, port);
        }

        public Socket connect() {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(remoteEndPoint);
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            socket.SendTimeout = 5000;
            return socket;
        }

        public int sendData(string data) {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            return socket.Send(buffer);
        }
    }
}
namespace OccupOS.CommonLibrary.NetworkControllers {

    using System.Net;

    public abstract class NetworkController {
        protected NetworkController(string hostname, ushort port) {
            this.hostname = hostname; this.port = port;
        }
        protected string hostname { get; set; }
        protected ushort port { get; set; }

        public abstract int SendData(string data);
        public abstract void Connect(string SSID, string key);
        public abstract void Disconnect();
    }
}
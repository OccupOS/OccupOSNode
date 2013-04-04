namespace OccupOS.CommonLibrary.NetworkControllers {

    using System.Net;
    using System;

    public abstract class NetworkController {
        protected NetworkController(string hostname, ushort port) {
            this.hostname = hostname; this.port = port;
        }
        protected string hostname { get; set; }
        protected ushort port { get; set; }

        public abstract int SendData(string data);
        public abstract Boolean Connect(string SSID, string key);
        public abstract void Disconnect();
    }
}
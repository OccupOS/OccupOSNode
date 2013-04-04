namespace OccupOSNode.Micro {

    using System.Threading;
    using Microsoft.SPOT;
    using Microsoft.SPOT.Hardware;
    using OccupOS.CommonLibrary;
    using OccupOS.CommonLibrary.Sensors;
    using SecretLabs.NETMF.Hardware.NetduinoPlus;
    using OccupOSNode.Micro.NetworkControllers.Arduino;
    using OccupOSNode.Micro.Sensors.Netduino;

    public class Program {
        private static readonly OutputPort outPrt = new OutputPort(Pins.ONBOARD_LED, false);

        public static void Main()
        {
            NetduinoNodeController controller = new NetduinoNodeController();
            controller.StartListening();

            NetduinoWirelessNetworkController ncontroller = new NetduinoWirelessNetworkController();
            ncontroller.ConnectToHost("RichyHotspot","occupos8","UrsaMinor",1333);
            
            SensorData sensorData = new SensorData();
            while (true) {
                int sensors = controller.GetSensorCount();
                if (sensors == 1) {
                    SensorData data = ((NetduinoWeatherShieldSensor)controller.GetSensor(0)).GetData();
                    if (!ncontroller.sendCommand(PacketFactory.CreatePacket(data)))
                        ncontroller.ConnectToHost("RichyHotspot", "occupos8", "UrsaMinor", 1333);
                    //string jsontest = PacketFactory.SerializeJSON(0, new SensorData[] {data});
                }
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
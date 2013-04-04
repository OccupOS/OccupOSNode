namespace OccupOSNode.Micro {

    using System.Threading;
    using Microsoft.SPOT;
    using Microsoft.SPOT.Hardware;
    using OccupOS.CommonLibrary;
    using OccupOS.CommonLibrary.Sensors;
    using SecretLabs.NETMF.Hardware.NetduinoPlus;
    using OccupOSNode.Micro.NetworkControllers.Netduino;
    using OccupOSNode.Micro.Sensors.Netduino;

    public class Program {
        private static readonly OutputPort outPrt = new OutputPort(Pins.ONBOARD_LED, false);

        public static void Main()
        {
            NetduinoNodeController controller = new NetduinoNodeController();
            controller.StartListening();

            NetduinoEthernetController ncontroller = new NetduinoEthernetController("UrsaMinor", 1333);
            //NetduinoWirelessNetworkController ncontroller = new NetduinoWirelessNetworkController("UrsaMinor", 1333);
            ncontroller.Connect("RichyHotspot","occupos8");
            
            SensorData sensorData = new SensorData();
            while (true) {
                int sensors = controller.GetSensorCount();
                if (sensors == 1) {
                    SensorData data = ((NetduinoWeatherShieldSensor)controller.GetSensor(0)).GetData();
                    if (ncontroller.SendData(PacketFactory.CreatePacket(data)) <= 0)
                        ncontroller.Connect("RichyHotspot", "occupos8");
                    //string jsontest = PacketFactory.SerializeJSON(0, new SensorData[] {data});
                }
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
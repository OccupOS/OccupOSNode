namespace OccupOSNode.Micro {

    using System.Threading;
    using Microsoft.SPOT;
    using Microsoft.SPOT.Hardware;
    using OccupOS.CommonLibrary;
    using OccupOS.CommonLibrary.Sensors;
    using SecretLabs.NETMF.Hardware.NetduinoPlus;
    using OccupOSNode.Micro.NetworkControllers.Netduino;
    using OccupOSNode.Micro.HardwareControllers.Netduino;
    using OccupOSNode.Micro.Sensors.Netduino;

    public class Program {
        private static readonly OutputPort outPrt = new OutputPort(Pins.ONBOARD_LED, false);

        public static void Main()
        {
            //NetduinoEthernetController ncontroller = new NetduinoEthernetController("192.168.0.3", 1333);
            NetduinoWirelessNetworkController ncontroller = new NetduinoWirelessNetworkController("UrsaMinor", 1333);
            while (!ncontroller.Connect("RichyHotspot", "occupos8")) { }

            NetduinoNodeController controller = new NetduinoNodeController(new NetduinoHardwareController(), ncontroller);
            controller.StartListening();
            
            SensorData sensorData = new SensorData();
            while (true) {
                int sensors = 0; /*controller.GetSensorCount();*/
                if (sensors == 1) {
                    SensorData data = new SensorData(); /*((NetduinoWeatherShieldSensor)controller.GetSensor(0)).GetData();*/
                    if (ncontroller.SendData(PacketFactory.CreatePacket(data)) <= 0)
                        ncontroller.Connect("RichyHotspot", "occupos8");
                    //string jsontest = PacketFactory.SerializeJSON(0, new SensorData[] {data});
                }
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
namespace OccupOSNode.Micro {

    using System.Threading;

    using Microsoft.SPOT;
    using Microsoft.SPOT.Hardware;

    using OccupOS.CommonLibrary;
    using OccupOS.CommonLibrary.Sensors;

    using SecretLabs.NETMF.Hardware.NetduinoPlus;
    using OccupOSNode.Micro.NetworkControllers.Arduino;
    using OccupOSNode.Micro.Sensors.Arduino;

    public class Program {
        private static readonly OutputPort outPrt = new OutputPort(Pins.ONBOARD_LED, false);

        public static void Main()
        {
            ArduinoNodeController controller = new ArduinoNodeController();
            controller.StartListening();

            ArduinoWirelessNetworkController ncontroller = new ArduinoWirelessNetworkController();
            ncontroller.ConnectToNetworkHost("testhoc","1234567890","JisangChoi-HP",1333);
            
            SensorData sensorData = new SensorData();
             
            while (true) {
                int sensors = controller.GetSensorCount();
                if (sensors == 1) {
                    SensorData data = ((ArduinoWeatherShieldSensor)controller.GetSensor(0)).GetData();
                    PacketFactory pfactory = new PacketFactory();
                    ncontroller.sendCommand(pfactory.CreatePacket(data));
                }
                System.Threading.Thread.Sleep(1000);
            }

            
            //var controller = new ArduinoEthernetController("192.168.1.127", 1333);

            /*if (controller.connect() == null) {
                blink();
            }
            else {
                blink1();
            }
            /*while (true) {
                if (controller.sendData("test") > 1) {
                    blink3();
                }
            }
            /*    // Create the manager / timers etc.
                while (true)
                {
               
                }*/
        }

        private static void blink() {
            outPrt.Write(true);
            Thread.Sleep(2000);
            outPrt.Write(false);
            Thread.Sleep(1000);
            outPrt.Write(true);
            Thread.Sleep(2000);
            outPrt.Write(false);
            Thread.Sleep(1000);
            outPrt.Write(true);
            Thread.Sleep(2000);
            outPrt.Write(false);
            Thread.Sleep(1000);
        }

        private static void blink1() {
            outPrt.Write(true);
            Thread.Sleep(2000);
            outPrt.Write(false);
            Thread.Sleep(1000);
            outPrt.Write(true);
            Thread.Sleep(2000);
            outPrt.Write(false);
            Thread.Sleep(1000);
        }

        private static void blink3() {
            outPrt.Write(true);
            Thread.Sleep(500);
            outPrt.Write(false);
            Thread.Sleep(500);
            outPrt.Write(true);
            Thread.Sleep(500);
            outPrt.Write(false);
            Thread.Sleep(500);
            outPrt.Write(true);
            Thread.Sleep(500);
            outPrt.Write(false);
            Thread.Sleep(1000);
        }
    }
}
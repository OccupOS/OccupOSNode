﻿using System.Threading;
using Microsoft.SPOT.Hardware;
using OccupOSNode.Micro.NetworkControllers.Arduino;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace OccupOSNode.Micro {
    public class Program {
        private static readonly OutputPort outPrt = new OutputPort(Pins.ONBOARD_LED, false);

        public static void Main() {
            blink();
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
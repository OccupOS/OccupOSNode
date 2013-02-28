using System.Threading;
using GHI.Premium.Net;
using Microsoft.SPOT;
using GT = Gadgeteer;

namespace GadgeteerDemo {
    public partial class Program {
        private readonly GT.Timer timer = new GT.Timer(2000);
        private NetworkController networkController;

        // This method is run when the mainboard is powered up or reset.   
        private void ProgramStarted() {
            wifi_RS21.DebugPrintEnabled = true;

            wifi_RS21.Interface.Open();

            NetworkInterfaceExtension.AssignNetworkingStackTo(wifi_RS21.Interface);

            wifi_RS21.Interface.NetworkInterface.EnableDhcp();
            wifi_RS21.Interface.NetworkInterface.EnableDynamicDns();
            //   wifi_RS21.Interface.NetworkInterface.EnableStaticIP("192.168.1.202", "255.255.255.0", "192.168.12.1");
            if (wifi_RS21.Interface.NetworkInterface.IsDhcpEnabled) {
                Debug.Print("DHCP enables");
            }

            Debug.Print("Scanning for WiFi networks");
            WiFiNetworkInfo[] wiFiNetworkInfo = wifi_RS21.Interface.Scan();
            if (wiFiNetworkInfo != null) {
                Debug.Print("Found WiFi network(s)");
                for (int i = 0; i < wiFiNetworkInfo.Length - 1; i++) {
                    if (wiFiNetworkInfo[i].SSID == "MorrisonN4") {
                        Debug.Print("Joining: " + wiFiNetworkInfo[i].SSID);
                        wifi_RS21.Interface.Join(wiFiNetworkInfo[i], "GadgeteerSucks");
                    }
                    else {
                        Debug.Print("Skipping: " + wiFiNetworkInfo[i].SSID);
                    }
                }

                Debug.Print(wifi_RS21.Interface.IsLinkConnected ? "Connection successful!" : "Connection failed!");
            }
            else {
                Debug.Print("Didn't find any WiFi networks");
            }

            networkController = new NetworkController("192.168.43.250", 1333);
            networkController.Connect();

            timer.Tick += timer_Tick;
            timer.Start();

            Debug.Print("Finished setup");
        }

        private void timer_Tick(GT.Timer timer) {
            var lightPercentage = (int)lightSensor.ReadLightSensorPercentage();
            Debug.Print(lightPercentage.ToString());

            networkController.SendData(lightPercentage.ToString());
            Debug.Print("(Data sent: light percentage - " + lightPercentage);

            Thread.Sleep(10000);
        }
    }
}
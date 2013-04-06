// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeController.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOS.CommonLibrary.NodeControllers
{
    using System;
    using System.Net.Sockets;
    using System.Threading;
    using OccupOS.CommonLibrary.HardwareControllers;
    using OccupOS.CommonLibrary.NetworkControllers;
    using OccupOS.CommonLibrary.Sensors;

    public abstract class NodeController
    {
        private static int DEFAULT_SEND_DELAY = 5000;
        private static int MAX_CONNECTION_ATTEMPTS = 20;

        protected int ID { get; set; }
        private Thread ds_thread = null;
        private DynamicSensorController dyn_controller = null;
        private HardwareController hardware_controller = null;
        private NetworkController network_controller = null;
        Thread buffer_thread = null;
        Thread upload_thread = null;
        private bool sending_active = false;
        private int send_delay;
        private int buffer_delay;
        private int buffer_max_size;

        public NodeController(int ID, HardwareController hardwareController, NetworkController networkController)
        {
            this.ID = ID;
            this.hardware_controller = hardwareController;
            this.network_controller = networkController;
        }

        public void DisableDynamicListening()
        {
            if (this.dyn_controller != null)
            {
                this.dyn_controller.Disable();
            }
        }

        public void EnableDynamicListening(ThreadPriority priority = ThreadPriority.Normal)
        {
            if (this.dyn_controller == null)
            {
                this.dyn_controller = new DynamicSensorController(this.hardware_controller);
            }

            if (this.ds_thread == null)
            {
                this.ds_thread = new Thread(new ThreadStart(this.dyn_controller.Run));
                this.ds_thread.Priority = priority;
                this.ds_thread.Start();
            }

            this.dyn_controller.Enable();
        }

        public void Start(int sendDelay, int bufferDelay = -1, int bufferMax = 40)
        {
            if (sending_active) throw new InvalidOperationException("NodeController already active");
            this.buffer_max_size = bufferMax;
            if (sendDelay > 0)
                this.send_delay = sendDelay;
            else
                send_delay = DEFAULT_SEND_DELAY;
            if (bufferDelay < 1)
                buffer_delay = sendDelay;
            else
                buffer_delay = bufferDelay;

            try {
                sending_active = true;
                buffer_thread = new Thread(new SensorPoller(hardware_controller, buffer_delay, buffer_max_size).Run);
                buffer_thread.Start();
                ConnectToTarget();
                upload_thread = new Thread(new ThreadStart(this.Run));
                upload_thread.Start();
            } catch (Exception e) {
                sending_active = false;
                if (buffer_thread != null) buffer_thread.Abort();
                if (upload_thread != null) upload_thread.Abort();
                throw e;
            }
        }

        private void Run() {
            SensorData[] readings = null;
            try {
                while (true) {
                    if (hardware_controller.GetSensorDataBufferCount() > 0) {
                        readings = hardware_controller.PollSensorReadings(0);
                        if (readings != null) {
                            foreach (SensorData data in readings) {
                                data.PollTime = System.DateTime.Now;
                            }
                            string jsondata = PacketFactory.SerializeJSON(ID, readings);
                            AttemptUpload(jsondata);
                        }
                    } else System.Threading.Thread.Sleep(send_delay);
                }
            } catch (Exception e) {
                sending_active = false;
                if (buffer_thread != null) buffer_thread.Abort();
                if (upload_thread != null) upload_thread.Abort();
                throw e;
            }
        }

        private void ConnectToTarget() {
            bool connected = false;
            for (int k = 0; k < MAX_CONNECTION_ATTEMPTS; k++) {
                connected = AttemptConnection();
                if (connected) break;
            }
            if (!connected) throw new SocketException(SocketError.HostUnreachable);
        }

        private bool AttemptConnection() {
            try {
                if (network_controller is WirelessNetworkController) {
                    ((WirelessNetworkController)network_controller).ConnectToWiFi();
                }
                network_controller.ConnectToSocket();
                return true;
            } catch (Exception e) {
                if (e is SocketException || e is ArgumentNullException) return false;
            }
            return false;
        }

        private void UploadToTarget(string data) {
            bool success = false;
            for (int k = 0; k < MAX_CONNECTION_ATTEMPTS; k++) {
                success = AttemptUpload(data);
                if (success) break;
            }
            if (!success) throw new SocketException(SocketError.HostUnreachable);
        }

        private bool AttemptUpload(string data) {
            try {
                network_controller.SendData(data);
                return true;
            } catch (Exception e) {
                if (e is SocketException || e is ArgumentNullException) {
                    return false;
                }
            }
            return false;
        }
    }
}
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
    using System.Reflection;
    using System.Threading;

    using OccupOS.CommonLibrary.HardwareControllers;
    using OccupOS.CommonLibrary.NetworkControllers;
    using OccupOS.CommonLibrary.Sensors;

    public abstract class NodeController
    {
        private const int DEFAULT_SEND_DELAY = 5000;

        private const int MAX_CONNECTION_ATTEMPTS = 50;

        private int bufferDelay;

        private int bufferMaxSize;

        private Thread bufferThread = null;

        private Thread dsThread = null;

        private DynamicSensorController dynController = null;

        private HardwareController hardwareController = null;

        private NetworkController networkController = null;

        private int sendDelay;

        private bool sendingActive = false;

        private Thread uploadThread = null;

        protected NodeController(int id, HardwareController hardwareController, NetworkController networkController)
        {
            this.ID = id;
            this.hardwareController = hardwareController;
            this.networkController = networkController;
        }

        protected int ID { get; set; }

        public void DisableDynamicListening()
        {
            if (this.dynController != null)
            {
                this.dynController.Disable();
            }
        }

        public void EnableDynamicListening(ThreadPriority priority = ThreadPriority.Normal)
        {
            if (this.dynController == null)
            {
                this.dynController = new DynamicSensorController(
                    this.hardwareController, Assembly.GetAssembly(this.GetType()));
            }

            if (this.dsThread == null)
            {
                this.dsThread = new Thread(new ThreadStart(this.dynController.Run));
                this.dsThread.Priority = priority;
                this.dsThread.Start();
            }

            this.dynController.Enable();
        }

        public void Start(int sendDelay, int bufferDelay = -1, int bufferMax = 40)
        {
            if (this.sendingActive)
            {
                throw new InvalidOperationException("NodeController already active");
            }

            this.bufferMaxSize = bufferMax;
            if (sendDelay > 0)
            {
                this.sendDelay = sendDelay;
            }
            else
            {
                this.sendDelay = DEFAULT_SEND_DELAY;
            }

            if (bufferDelay < 1)
            {
                this.bufferDelay = sendDelay;
            }
            else
            {
                this.bufferDelay = bufferDelay;
            }

            try
            {
                this.sendingActive = true;
                this.bufferThread =
                    new Thread(new SensorPoller(this.hardwareController, this.bufferDelay, this.bufferMaxSize).Run);
                this.bufferThread.Start();
                this.ConnectToTarget();
                this.uploadThread = new Thread(new ThreadStart(this.Run));
                this.uploadThread.Start();
            }
            catch (Exception e)
            {
                this.sendingActive = false;
                if (this.bufferThread != null)
                {
                    this.bufferThread.Abort();
                }

                if (this.uploadThread != null)
                {
                    this.uploadThread.Abort();
                }

                throw e;
            }
        }

        private bool AttemptConnection()
        {
            try
            {
                if (this.networkController is WirelessNetworkController)
                {
                    ((WirelessNetworkController)this.networkController).ConnectToWiFi();
                }

                this.networkController.ConnectToSocket();
                return true;
            }
            catch (Exception e)
            {
                if (e is SocketException || e is ArgumentNullException)
                {
                    return false;
                }
            }

            return false;
        }

        private bool AttemptUpload(string data)
        {
            try
            {
                this.networkController.SendData(data);
                return true;
            }
            catch (Exception e)
            {
                if (e is SocketException || e is ArgumentNullException)
                {
                    return false;
                }
            }

            return false;
        }

        private void ConnectToTarget()
        {
            bool connected = false;
            for (int k = 0; k < MAX_CONNECTION_ATTEMPTS; k++)
            {
                connected = this.AttemptConnection();
                if (connected)
                {
                    break;
                }
            }

            if (!connected)
            {
                throw new SocketException(SocketError.HostUnreachable);
            }
        }

        private void Run()
        {
            SensorData[] readings = null;
            try
            {
                while (true)
                {
                    if (this.hardwareController.GetSensorDataBufferCount() > 0)
                    {
                        readings = this.hardwareController.PollSensorReadings(0);
                        if (readings != null)
                        {
                            foreach (SensorData data in readings)
                            {
                                data.PollTime = System.DateTime.Now;
                            }

                            string jsondata = PacketFactory.SerializeJSON(this.ID, readings);
                            this.UploadToTarget(jsondata);
                        }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(this.sendDelay);
                    }
                }
            }
            catch (Exception e)
            {
                this.sendingActive = false;
                if (this.bufferThread != null)
                {
                    this.bufferThread.Abort();
                }

                if (this.uploadThread != null)
                {
                    this.uploadThread.Abort();
                }

                throw e;
            }
        }

        private void UploadToTarget(string data)
        {
            bool success = false;
            for (int k = 0; k < MAX_CONNECTION_ATTEMPTS; k++)
            {
                success = this.AttemptUpload(data);
                if (success)
                {
                    break;
                }
            }

            if (!success)
            {
                throw new SocketException(SocketError.HostUnreachable);
            }
        }
    }
}
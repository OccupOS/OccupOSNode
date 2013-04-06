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
    using System.Threading;

    using OccupOS.CommonLibrary.HardwareControllers;
    using OccupOS.CommonLibrary.NetworkControllers;

    public abstract class NodeController
    {
        private Thread ds_thread = null;
        private DynamicSensorController dyn_controller = null;
        private HardwareController hardware_controller = null;
        private NetworkController network_controller = null;

        public NodeController(HardwareController hardwareController, NetworkController networkController)
        {
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
                this.ds_thread = new Thread(this.dyn_controller.Run);
                this.ds_thread.Priority = priority;
                this.ds_thread.Start();
            }

            this.dyn_controller.Enable();
        }

        public void Run()
        {
            // todo: CreateSensorPoller, poll buffer, timestamp poll, serialize and send via networkController
            // HardwareController monitors Sensor Arrays, may need to make Arrays threadsafe
        }
    }
}
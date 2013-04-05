using System;
using System.Collections;
using OccupOS.CommonLibrary.HardwareControllers;
using OccupOS.CommonLibrary.NetworkControllers;
using System.Threading;

namespace OccupOS.CommonLibrary.NodeControllers {

    public abstract class NodeController {
        private HardwareController hardware_controller = null;
        private NetworkController network_controller = null;
        private DynamicSensorController dyn_controller = null;
        private Thread ds_thread = null;

        public NodeController(HardwareController hardwareController, NetworkController networkController) {
            this.hardware_controller = hardwareController;
            this.network_controller = networkController;
        }

        public void StartListening(ThreadPriority priority = ThreadPriority.Normal) {
            if (dyn_controller == null) {
                dyn_controller = new DynamicSensorController(hardware_controller);
            }
            if (ds_thread == null) {
                ds_thread = new Thread(dyn_controller.Start);
                ds_thread.Priority = priority;
                ds_thread.Start();
            }
            dyn_controller.Enable();
        }

        public void StopListening() {
            if (dyn_controller != null) {
                dyn_controller.Disable();
            }
        }
    }
}
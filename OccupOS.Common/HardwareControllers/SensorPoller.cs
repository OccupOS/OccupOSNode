namespace OccupOS.CommonLibrary.HardwareControllers {

    using System;
    using System.Collections;
    using System.Threading;
    using OccupOS.CommonLibrary.Sensors;

    public class SensorPoller {

        private int delay_time;
        private int max_buffer_size;
        private HardwareController hw_controller;

        public SensorPoller(HardwareController hardwareController, int delay = 2000, int maxBuffer = 50) {
            if (delay < 0 || maxBuffer < 1) throw new ArgumentException();
            this.hw_controller = hardwareController;
            this.delay_time = delay;
            this.max_buffer_size = maxBuffer;
        }

        public void Run() {
            while (true) {
                if (hw_controller.GetSensorDataBufferCount() >= max_buffer_size)
                    hw_controller.RemoveSensorReadings(0);
                hw_controller.AddSensorReadings(SampleSensorData());
                System.Threading.Thread.Sleep(delay_time);
            }
        }

        private SensorData[] SampleSensorData() {
            ArrayList sample = hw_controller.GetAllSensors();
            SensorData[] result = new SensorData[sample.Count];
            for (int k = 0; k < sample.Count; k++) {
                result[k] = ((Sensor) sample[k]).GetData();
            }
            return result;
        }
    }
}

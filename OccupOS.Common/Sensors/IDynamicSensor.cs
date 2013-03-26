namespace OccupOS.CommonLibrary.Sensors {
    public interface IDynamicSensor {
        int GetMaxSensors();
        int GetDeviceCount();
        ConnectionStatus GetConnectionStatus();
        void Connect();
        void Disconnect();
    }
}

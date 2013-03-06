namespace OccupOS.CommonLibrary.Sensors {
    public interface IDynamicSensor {
        int GetDeviceCount();
        ConnectionStatus GetConnectionStatus();
        void Connect();
        void Disconnect();
    }
}

using System;
using System.Threading;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace OccupOSNode.Micro.Sensors.Arduino {
    public class ArduinoWeatherShieldDriver {
        public static Cpu.Pin DEFAULTCLOCK_PIN = Cpu.Pin.GPIO_Pin7;
        public static Cpu.Pin DEFAULTIODATA_PIN = Cpu.Pin.GPIO_Pin2;
        public static Byte DEFAULTADDRESS = 0x01;

        public enum units {
            TEMPERATURE,
            PRESSURE,
            HUMIDITY
        };

        public enum sample {
            SAMPLE_ONE = 0x00,
            SAMPLE_TWO = 0x01,
            SAMPLE_THREE = 0x02,
            SAMPLE_FOUR = 0x03,
            SAMPLE_FIVE = 0x04,
            SAMPLE_SIX = 0x05,
            SAMPLE_SEVEN = 0x06,
            SAMPLE_EIGHT = 0x07,
            LAST_SAMPLE = 0x80,
            AVG_SAMPLE = 0x81
        };

        private enum commands {
            CMD_UNKNOWN = 0x00,
            CMD_SETADDRESS = 0x01,
            CMD_ECHO_PAR = 0x02,
            CMD_SET_SAMPLETIME = 0x03,
            CMD_GETTEMP_C_AVG = 0x04,
            CMD_GETTEMP_C_RAW = 0x05,
            CMD_GETPRESS_AVG = 0x06,
            CMD_GETPRESS_RAW = 0x07,
            CMD_GETHUM_AVG = 0x08,
            CMD_GETHUM_RAW = 0x09,
        };

        private static int RXCOMMANDPOS = 3;
        private static int RXPAR1POS = 2;
        private static int RXPAR2POS = 1;
        private static int RXPAR3POS = 0;
        private static int RXBUFFERLENGTH = 4;

        private Byte m_deviceAddress = DEFAULTADDRESS;
        private Byte[] m_tempBuffer = new Byte[RXBUFFERLENGTH];
        private bool averageValuesValid = false;
        private bool averageValuesChecked = false;

        public ArduinoWeatherShieldDriver() {
            ArduinoNodeController.AttemptSetOutputPort(DEFAULTCLOCK_PIN, false);
            ArduinoNodeController.AttemptSetTristatePort(DEFAULTIODATA_PIN, false, true, Port.ResistorMode.Disabled);
            this.resetConnection();
        }

        public ArduinoWeatherShieldDriver(Cpu.Pin clockPin, Cpu.Pin dataPin, Byte deviceAddress) {
            ArduinoNodeController.AttemptSetOutputPort(clockPin, false);
            ArduinoNodeController.AttemptSetTristatePort(dataPin, false, true, Port.ResistorMode.Disabled);
            this.m_deviceAddress = deviceAddress;
            this.resetConnection();
        }

        /* Initialize the connection with the WeatherShield1 */
        public void resetConnection() {
            ArduinoNodeController.GetOutputPort().Write(false);

            /* We start sending a high level bit (start bit) */
            if (!ArduinoNodeController.GetTristatePort().Active)
                ArduinoNodeController.GetTristatePort().Active = true;
            ArduinoNodeController.GetTristatePort().Write(true);
            this.pulseClockPin();

            /* Then we send a sequence of "fake" low level bits */
            for (int ucN = 0; ucN < 200; ucN++) {
                ArduinoNodeController.GetTristatePort().Write(false);
                this.pulseClockPin();
            }
        }

        /* Assign a new address to the WeatherShield1 */
        public void setBoardAddress(Byte newAddress) {
            this.sendCommand(commands.CMD_SETADDRESS, newAddress);
        }

        /* Set a new sample time (in seconds from 1 to 256) */
        public void setSampleTime(Byte seconds) {
            this.sendCommand(commands.CMD_SET_SAMPLETIME, seconds);
        }

        /* Send back the parameter through the WeatherShield1 */
        public Byte echo(Byte parameter) {
            this.sendCommand(commands.CMD_ECHO_PAR, parameter);
            if (this.readAnswer(commands.CMD_ECHO_PAR))
                return this.m_tempBuffer[RXPAR1POS];

            return 0;
        }

        /* Read an averaged value of specified unit (or MinValue if fails) */
        public float readAveragedValue(units unitType) {
            float result = float.MinValue;
            commands command = commands.CMD_UNKNOWN;

            switch (unitType) {
                case units.TEMPERATURE:
                    command = commands.CMD_GETTEMP_C_AVG;
                    break;

                case units.HUMIDITY:
                    command = commands.CMD_GETHUM_AVG;
                    break;

                case units.PRESSURE:
                    command = commands.CMD_GETPRESS_AVG;
                    break;
            }

            this.sendCommand(command, 0);
            if (this.readAnswer(command))
                result = this.decodeFloatValue();

            return result;
        }

        /* Read a specific sample for a specified unit in a RAW format */
        /* Returns MinValue if fails */
        public short readRawValue(units unitType, sample sampleNum) {
            short result = short.MinValue;
            commands command = commands.CMD_UNKNOWN;

            switch (unitType) {
                case units.TEMPERATURE:
                    command = commands.CMD_GETTEMP_C_RAW;
                    break;

                case units.HUMIDITY:
                    command = commands.CMD_GETHUM_RAW;
                    break;

                case units.PRESSURE:
                    command = commands.CMD_GETPRESS_RAW;
                    break;
            }

            this.sendCommand(command, (Byte)sampleNum);
            if (this.readAnswer(command))
                result = this.decodeShortValue();

            return result;
        }

        /* Averaged values are calculated with last 8 raw samples */
        /* This function returns true if the shield contains at least */
        /* 8 valid raw samples in the buffer */
        public bool averageValuesReady() {
            if (!this.averageValuesChecked || !this.averageValuesValid) {
                this.averageValuesValid = false;

                /* Check for valid connection */
                if (this.echo(0x55) != 0x55)
                    return this.averageValuesValid;

                /* Read the last 8 raw temperature samples 
                 ans check they're not zero */
                this.averageValuesValid = true;
                for (int n = 0; n < 8; n++) {
                    short value = this.readRawValue(units.HUMIDITY, (sample)n);
                    this.averageValuesValid &= (value != 0);
                }

                this.averageValuesChecked = true;
            }

            return this.averageValuesValid;
        }

        /* Generate a clock pulse */
        private void pulseClockPin() {
            ArduinoNodeController.GetOutputPort().Write(true);
            Thread.Sleep(5);
            ArduinoNodeController.GetOutputPort().Write(false);
            Thread.Sleep(5);
        }

        /* Send a byte through the synchronous serial line */
        private void sendByte(Byte ucData) {
            for (int n = 0; n < 8; n++) {

                bool bit = (ucData & 0x80) != 0;
                ArduinoNodeController.GetTristatePort().Write(bit);

                this.pulseClockPin();
                ucData = (Byte)(ucData << 1);
            }
        }

        /* Read a byte from the synchronous serial line */
        private Byte readByte() {
            Byte result = 0;

            for (int n = 0; n < 8; n++) {

                ArduinoNodeController.GetOutputPort().Write(true);
                Thread.Sleep(5);

                result = (Byte)(result << 1);
                bool input = ArduinoNodeController.GetTristatePort().Read();
                result |= (Byte)((input) ? 1 : 0);

                ArduinoNodeController.GetOutputPort().Write(false);
                Thread.Sleep(5);
            }

            return result;
        }

        /* Send a command request to the WeatherShield1 */
        private void sendCommand(commands command, Byte parameter) {
            /* We start sending the first high level bit */
            if (!ArduinoNodeController.GetTristatePort().Active)
                ArduinoNodeController.GetTristatePort().Active = true;
            ArduinoNodeController.GetTristatePort().Write(true);
            this.pulseClockPin();

            /* The first byte is always 0xAA... */
            this.sendByte(0xAA);

            /* ... then is the address... */
            this.sendByte(this.m_deviceAddress);

            /* ... then is the command ... */
            this.sendByte((Byte)command);

            /* ... and the parameter ... */
            this.sendByte(parameter);

            /* And this is the last low level bit required by the protocol */
            ArduinoNodeController.GetTristatePort().Write(false);
            this.pulseClockPin();
        }

        /* Read the answer back from the Weather Shield 1 and fill the provided
        buffer with the result. Depending on the type of command associated
        to this answer the buffer contents should be properly decoded.
        The function returns true if the read answer contain the expected 
        command */
        private bool readAnswer(commands command) {
            ArduinoNodeController.GetTristatePort().Active = false;

            for (int n = RXBUFFERLENGTH; n > 0; n--)
                this.m_tempBuffer[n - 1] = this.readByte();

            ArduinoNodeController.GetTristatePort().Active = true;

            return (this.m_tempBuffer[RXCOMMANDPOS] == (Byte)command);
        }

        /* Convert read bytes in a float value */
        private float decodeFloatValue() {

            Byte cMSD = this.m_tempBuffer[RXPAR1POS];
            Byte cLSD = this.m_tempBuffer[RXPAR2POS];

            float fVal = cMSD + (((float)cLSD) / 100.0f);

            return fVal;
        }

        /* Convert read bytes in a short value */
        private short decodeShortValue() {

            Byte cMSD = this.m_tempBuffer[RXPAR1POS];
            Byte cLSD = this.m_tempBuffer[RXPAR2POS];

            short shResult = (short)((cMSD << 8) | cLSD);

            return shResult;
        }
    }
}
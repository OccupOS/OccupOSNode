using System;
using OccupOS.CommonLibrary.Sensors;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Reflection;
using System.Threading;

// Improved implementation of the MLX90620 driver for Micro .NET Framework
// Based on the Arduino implementation by IlBaboomba and I2C repeated start methods by Chris Walker @secretlabs
// @author Jisang Choi <7517choi@armymail.mod.uk> 

namespace OccupOSNode.Micro.Netduino.Sensors.Arduino {
    class ArduinoMLX90620Driver {

        int freq = 16;

        int[] IRDATA = new int[64];
        byte CFG_LSB, CFG_MSB, PTAT_LSB, PTAT_MSB, CPIX_LSB, CPIX_MSB, PIX_LSB, PIX_MSB;
        int PIX, v_th, CPIX;
        float ta, to, emissivity, k_t1, k_t2;
        float[] temperatures = new float[64];
        int count = 0;
        uint PTAT;
        int a_cp, b_cp, tgc, b_i_scale;

        int[] a_ij = new int[64];
        int[] b_ij = new int[64];

        double[] alpha_ij = new double[64];
        float[] v_ir_tgc_comp = new float[64];

        I2CDevice mlx90620_0xC0 = new I2CDevice(new I2CDevice.Configuration(0xC0, 400));
        I2CDevice mlx90620_0xC1 = new I2CDevice(new I2CDevice.Configuration(0xC1, 400));
        I2CDevice mlx90620_0xA0 = new I2CDevice(new I2CDevice.Configuration(0xA0, 400));
        I2CDevice mlx90620_0xA1 = new I2CDevice(new I2CDevice.Configuration(0xA1, 400));

        public void config_MLX90620_16Hz() {
            byte[] toWrite = { 0x03, 0xB5, 0x0A, 0x1F, 0x74 };
            i2c_write(toWrite, 0xC0, mlx90620_0xC0);
        }

        public void read_EEPROM_MLX90620() {
            byte[] EEPROM_DATA = new byte[256];
            byte[] toWrite = { 0x00 };
            i2c_write(toWrite, 0xA0, mlx90620_0xA0);
            i2c_readAck(EEPROM_DATA, 0xA1, mlx90620_0xA1);
            byte[] toRead = new byte[256];
            for(int i = 0; i <= 256; i++){
                EEPROM_DATA[i] = toRead[i];
            }
            varInitialization(EEPROM_DATA);
            write_trimming_value(EEPROM_DATA[247]);
        }

        public void write_trimming_value(byte val) {
            const byte OFFSET = 0xAA;
            byte newAddress = (byte) (val - OFFSET);

            byte[] toWrite = { 0x04, newAddress, val, 0x56, 0x00 };
            i2c_write(toWrite, 0xC0, mlx90620_0xC0);
        }

        public void calculate_TA() {
            throw new NotImplementedException();
        }

        public void calculate_TO() {
            throw new NotImplementedException();
        }

        public void read_IR_ALL_MLX90620() {
            byte[] toWrite = { 0x02, 0x00, 0x01, 0x40 };
            i2c_write(toWrite, 0xC0, mlx90620_0xC0);
            byte[] toRead = { 0, 0 }; //Reads low byte and high byte
            for (int i = 0; i <= 63; i++) {
                i2c_readAck(toRead, 0xC1, mlx90620_0xC1);
                PIX_LSB = toRead[0];
                PIX_MSB = toRead[1];
                IRDATA[i] = (PIX_MSB << 8) + PIX_LSB;
            }
        }

        public void read_PTAT_Reg_MLX09620() {
            byte[] toWrite = { 0x02, 0x90, 0x00, 0x01 };
            i2c_write(toWrite, 0xC0, mlx90620_0xC0);
            byte[] toRead = { 0, 0 };
            i2c_readAck(toRead, 0xC1, mlx90620_0xC1);
            PTAT_LSB = toRead[0];
            PTAT_MSB = toRead[1];
            PTAT = ((uint)PTAT_MSB << 8) + PTAT_LSB;
        }

        public void read_CPIX_Reg_MLX90620() {
            byte[] toWrite = { 0x02, 0x91, 0x00, 0x01 };
            i2c_write(toWrite, 0xC0, mlx90620_0xC0);
            byte[] toRead = { 0, 0 };
            i2c_readAck(toRead, 0xC1, mlx90620_0xC1);
            CPIX_LSB = toRead[0];
            CPIX_MSB = toRead[1];
            CPIX = (CPIX_MSB << 8) + CPIX_LSB;
        }

        public void read_Config_Reg_MLX90620() {
            byte[] toWrite = { 0x02, 0x92, 0x00, 0x01 };
            i2c_write(toWrite, 0xC0, mlx90620_0xC0);
            byte[] toRead = { 0, 0 };
            i2c_readAck(toRead, 0xC1, mlx90620_0xC1);
            CFG_LSB = toRead[0];
            CFG_MSB = toRead[1];
        }

        public void check_Config_Reg_MLX90620() {
            read_Config_Reg_MLX90620();
            if (!((CFG_MSB & 0x04) == 0x04)) {
                config_MLX90620_16Hz();
            }
        }

        public void varInitialization(byte[] EEPROM_DATA) {
            v_th = (EEPROM_DATA[219] << 8) + EEPROM_DATA[218];
            k_t1 = ((EEPROM_DATA[221] << 8) + EEPROM_DATA[220]) / 1024;
            k_t2 = ((EEPROM_DATA[223] << 8) + EEPROM_DATA[222]) / 1048576;

            a_cp = EEPROM_DATA[212];
            if (a_cp > 127) {
                a_cp = a_cp - 256;
            }

            b_cp = EEPROM_DATA[213];
            if (b_cp > 127) {
                b_cp = b_cp - 256;
            }
            tgc = EEPROM_DATA[216];
            if (tgc > 127) {
                tgc = tgc - 256;
            }

            b_i_scale = EEPROM_DATA[217];
            emissivity = (((uint)EEPROM_DATA[229] << 8) + EEPROM_DATA[228]) / 32768;

            for (int i = 0; i <= 63; i++) {
                a_ij[i] = EEPROM_DATA[i];
                if (a_ij[i] > 127) {
                    a_ij[i] = a_ij[i] - 256;
                }
                b_ij[i] = EEPROM_DATA[64 + i];
                if (b_ij[i] > 127) {
                    b_ij[i] = b_ij[i] - 256;
                }
            }
            //Calculates EEPROM alpha_ij constant values directly instead of using a spreadsheet. Taken from code from rmie.
            double da0_scale = System.Math.Pow(2, -EEPROM_DATA[0xe3]);
            double alpha_const = (double)(((uint)EEPROM_DATA[0xe1] << 8) + (uint)EEPROM_DATA[0xe0] * (System.Math.Pow(2, -EEPROM_DATA[0xe2])));
            for(int i=0; i<=63; i++) {
                double alpha_var = (double)EEPROM_DATA[0x80 + i] * da0_scale;
                alpha_ij[i] = (alpha_const + alpha_var);
            }
        }

        private void i2c_write(byte[] toWrite, byte address, I2CDevice startAddress) {
            I2CDevice.I2CTransaction[] reading = new I2CDevice.I2CTransaction[]{
                CreateWriteTransaction(toWrite, address, 1)
            };
            int bytesRead = startAddress.Execute(reading, 100);
        }

        /// <summary>
        /// The i 2 c_read ack.
        /// </summary>
        /// <param name="toRead">
        /// The to read.
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="startAddress">
        /// The start address.
        /// </param>
        private void i2c_readAck(byte[] toRead, byte address, I2CDevice startAddress) 
        {
            I2CDevice.I2CTransaction[] reading = new I2CDevice.I2CTransaction[]{
                CreateReadTransaction(toRead, address, 1)
            };
            int bytesRead = startAddress.Execute(reading, 100);
        }

        //Overridden read/write methods for I2C
        static I2CDevice.I2CWriteTransaction CreateWriteTransaction(byte[] buffer, uint addr, byte addrSz) {
            I2CDevice.I2CWriteTransaction writeTransaction = I2CDevice.CreateWriteTransaction(buffer);
            Type writeTransactionType = typeof(I2CDevice.I2CWriteTransaction);

            FieldInfo fieldInfo = writeTransactionType.GetField("Custom_InternalAddress",
                BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(writeTransaction, addr);

            fieldInfo = writeTransactionType.GetField("Custom_InternalAddressSize",
                BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(writeTransaction, addrSz);

            return writeTransaction;
        }

        static I2CDevice.I2CReadTransaction CreateReadTransaction(byte[] buffer, uint addr, byte addrSz) {
            I2CDevice.I2CReadTransaction readTransaction = I2CDevice.CreateReadTransaction(buffer);
            Type readTransactionType = typeof(I2CDevice.I2CReadTransaction);

            FieldInfo fieldInfo = readTransactionType.GetField("Custom_InternalAddress",
                BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(readTransaction, addr);

            fieldInfo = readTransactionType.GetField("Custom_InternalAddressSize",
                BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(readTransaction, addrSz);

            return readTransaction;

        }
    }
}

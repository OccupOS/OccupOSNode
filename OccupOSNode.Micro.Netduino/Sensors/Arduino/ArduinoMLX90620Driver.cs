using System;
using OccupOS.CommonLibrary.Sensors;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Reflection;
using System.Threading;
using System.Math;

// Improved implementation of the MLX90620 driver for Micro .NET Framework
// Based on the Arduino implementation by IlBaboomba and I2C repeated start methods by Chris Walker @secretlabs
// @author Jisang Choi <7517choi@armymail.mod.uk> 

namespace OccupOSNode.Micro.Netduino.Sensors.Arduino {
    class ArduinoMLX90620Driver : I2CDevice {

        int freq = 16;

        fixed int IRDATA[64];
        byte CFG_LSB, CFG_MSB, PTAT_LSB, PTAT_MSB, CPIX_LSB, CPIX_MSB, PIX_LSB, PIX_MSB;
        int PIX, v_th, CPIX;
        float ta, to, emissivity, k_t1, k_t2;
        fixed float temperatures[64];
        int count = 0;
        uint PTAT;
        int a_cp, b_cp, tgc, b_i_scale;

        fixed int a_ij[64];
        fixed int b_ij[64];

        fixed double alpha_ij[64];
        fixed float v_ir_tgc_comp[64];

        I2CDevice mlx90620 = new I2CDevice(new I2CDevice.Configuration(0xC0, 400));

        public void config_MLX90620_16Hz() {

            i2c_write(0x03);
            i2c_write(0xB5);
            i2c_write(0x0A);
            i2c_write(0x1F);
            i2c_write(0x74);

        }

        public void read_EEPROM_MLX90620() {
            throw new NotImplementedException();
        }

        public void write_trimming_value(byte val) {
            i2c_write(0x04);
            byte newAddress = new byte();
            int d = val - 0xAA;
            String b = d.ToString();
            newAddress = Convert.ToByte(b);
            i2c_write(newAddress);
            i2c_write(val);
            i2c_write(0x56);
            i2c_write(0x00);
        }

        public void calculate_TA() {
            throw new NotImplementedException();
        }

        public void calculate_TO() {
            throw new NotImplementedException();
        }

        public void read_IR_ALL_MLX90620() {
            i2c_write(0x02);
            i2c_write(0x00);
            i2c_write(0x01);
            i2c_write(0x40);
            i2c_write(0x40);
            for (int i = 0; i <= 63; i++) {
            }
        }

        private void i2c_write(byte address) {

            byte[] cmd = { address };
            I2CDevice.I2CTransaction[] reading = new I2CDevice.I2CTransaction[]{
                CreateWriteTransaction(cmd, address, 1)
            };
            int bytesRead = mlx90620.Execute(reading, 100);
        }

        private int i2c_readAck(byte address) {

            byte[] cmd = { address };
            I2CDevice.I2CTransaction[] reading = new I2CDevice.I2CTransaction[]{
                CreateReadTransaction(cmd, address, 1)
            };
            int bytesRead = mlx90620.Execute(reading, 100);
            return bytesRead;
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

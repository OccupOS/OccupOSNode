// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetduinoMLX90620Driver.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

 // Improved implementation of the MLX90620 driver for Micro .NET Framework
// Based on the Arduino implementation by IlBaboomba and I2C repeated start methods by Chris Walker @secretlabs
// @author Jisang Choi <7517choi@armymail.mod.uk> 
namespace OccupOSNode.Micro.Netduino.Sensors.Netduino
{
    using System;
    using System.Reflection;

    using Microsoft.SPOT.Hardware;

    internal class NetduinoMLX90620Driver
    {
        private byte CFG_LSB;

        private byte CFG_MSB;

        private int CPIX;

        private byte CPIX_LSB;

        private byte CPIX_MSB;

        private int[] IRDATA = new int[64];

        private int PIX;

        private byte PIX_LSB;

        private byte PIX_MSB;

        private uint PTAT;

        private byte PTAT_LSB;

        private byte PTAT_MSB;

        private int a_cp;

        private int[] a_ij = new int[64];

        private double[] alpha_ij = new double[64];

        private int b_cp;

        private int b_i_scale;

        private int[] b_ij = new int[64];

        private int count = 0;

        private float emissivity;

        private int freq = 16;

        private float k_t1;

        private float k_t2;

        private I2CDevice mlx90620_0xA0 = new I2CDevice(new I2CDevice.Configuration(0xA0, 400));

        private I2CDevice mlx90620_0xA1 = new I2CDevice(new I2CDevice.Configuration(0xA1, 400));

        private I2CDevice mlx90620_0xC0 = new I2CDevice(new I2CDevice.Configuration(0xC0, 400));

        private I2CDevice mlx90620_0xC1 = new I2CDevice(new I2CDevice.Configuration(0xC1, 400));

        private double ta;

        private float[] temperatures = new float[64];

        private int tgc;

        private float to;

        private float[] v_ir_tgc_comp = new float[64];

        private int v_th;

        public void calculate_TA()
        {
            this.ta = (-this.k_t1
                       + System.Math.Sqrt(
                           System.Math.Pow(2, this.k_t1) - (4 * this.k_t2 * (this.v_th - (float)this.PTAT))))
                      / (2 * this.k_t2) + 25;
        }

        public void calculate_TO()
        {
            double v_cp_off_comp = (float)this.CPIX
                                   - (this.a_cp + (this.b_cp / System.Math.Pow(2, this.b_i_scale)) * (this.ta - 25));
                
                // this is needed only during the to calculation, so I declare it here.
            for (int i = 0; i < 64; i++)
            {
                this.v_ir_tgc_comp[i] =
                    (float)
                    (this.IRDATA[i]
                     - (this.a_ij[i] + (float)(this.b_ij[i] / System.Math.Pow(2, this.b_i_scale)) * (this.ta - 25))
                     - (((float)this.tgc / 32) * v_cp_off_comp));

                // v_ir_comp[i]= v_ir_tgc_comp[i] / emissivity; //removed to save SRAM, since emissivity in my case is equal to 1.
                // temperatures[i] = sqrt(sqrt((v_ir_comp[i]/alpha_ij[i]) + pow((ta + 273.15),4))) - 273.15;
                this.temperatures[i] =
                    (float)
                    (System.Math.Sqrt(
                        System.Math.Sqrt(
                            (this.v_ir_tgc_comp[i] / this.alpha_ij[i]) + System.Math.Pow(this.ta + 273.15, 4))) - 273.15);
                    
                    // edited to work with v_ir_tgc_comp instead of v_ir_comp
            }
        }

        public void check_Config_Reg_MLX90620()
        {
            this.read_Config_Reg_MLX90620();
            if (!((this.CFG_MSB & 0x04) == 0x04))
            {
                this.config_MLX90620_16Hz();
            }
        }

        public void config_MLX90620_16Hz()
        {
            byte[] toWrite = { 0x03, 0xB5, 0x0A, 0x1F, 0x74 };
            this.i2c_write(toWrite, 0xC0, this.mlx90620_0xC0);
        }

        public void read_CPIX_Reg_MLX90620()
        {
            byte[] toWrite = { 0x02, 0x91, 0x00, 0x01 };
            this.i2c_write(toWrite, 0xC0, this.mlx90620_0xC0);
            byte[] toRead = { 0, 0 };
            this.i2c_readAck(toRead, 0xC1, this.mlx90620_0xC1);
            this.CPIX_LSB = toRead[0];
            this.CPIX_MSB = toRead[1];
            this.CPIX = (this.CPIX_MSB << 8) + this.CPIX_LSB;
        }

        public void read_Config_Reg_MLX90620()
        {
            byte[] toWrite = { 0x02, 0x92, 0x00, 0x01 };
            this.i2c_write(toWrite, 0xC0, this.mlx90620_0xC0);
            byte[] toRead = { 0, 0 };
            this.i2c_readAck(toRead, 0xC1, this.mlx90620_0xC1);
            this.CFG_LSB = toRead[0];
            this.CFG_MSB = toRead[1];
        }

        public void read_EEPROM_MLX90620()
        {
            byte[] EEPROM_DATA = new byte[256];
            byte[] toWrite = { 0x00 };
            this.i2c_write(toWrite, 0xA0, this.mlx90620_0xA0);
            this.i2c_readAck(EEPROM_DATA, 0xA1, this.mlx90620_0xA1);
            byte[] toRead = new byte[256];
            for (int i = 0; i <= 256; i++)
            {
                EEPROM_DATA[i] = toRead[i];
            }

            this.varInitialization(EEPROM_DATA);
            this.write_trimming_value(EEPROM_DATA[247]);
        }

        public void read_IR_ALL_MLX90620()
        {
            byte[] toWrite = { 0x02, 0x00, 0x01, 0x40 };
            this.i2c_write(toWrite, 0xC0, this.mlx90620_0xC0);
            byte[] toRead = { 0, 0 }; // Reads low byte and high byte
            for (int i = 0; i <= 63; i++)
            {
                this.i2c_readAck(toRead, 0xC1, this.mlx90620_0xC1);
                this.PIX_LSB = toRead[0];
                this.PIX_MSB = toRead[1];
                this.IRDATA[i] = (this.PIX_MSB << 8) + this.PIX_LSB;
            }
        }

        public void read_PTAT_Reg_MLX09620()
        {
            byte[] toWrite = { 0x02, 0x90, 0x00, 0x01 };
            this.i2c_write(toWrite, 0xC0, this.mlx90620_0xC0);
            byte[] toRead = { 0, 0 };
            this.i2c_readAck(toRead, 0xC1, this.mlx90620_0xC1);
            this.PTAT_LSB = toRead[0];
            this.PTAT_MSB = toRead[1];
            this.PTAT = ((uint)this.PTAT_MSB << 8) + this.PTAT_LSB;
        }

        public void varInitialization(byte[] EEPROM_DATA)
        {
            this.v_th = (EEPROM_DATA[219] << 8) + EEPROM_DATA[218];
            this.k_t1 = ((EEPROM_DATA[221] << 8) + EEPROM_DATA[220]) / 1024;
            this.k_t2 = ((EEPROM_DATA[223] << 8) + EEPROM_DATA[222]) / 1048576;

            this.a_cp = EEPROM_DATA[212];
            if (this.a_cp > 127)
            {
                this.a_cp = this.a_cp - 256;
            }

            this.b_cp = EEPROM_DATA[213];
            if (this.b_cp > 127)
            {
                this.b_cp = this.b_cp - 256;
            }

            this.tgc = EEPROM_DATA[216];
            if (this.tgc > 127)
            {
                this.tgc = this.tgc - 256;
            }

            this.b_i_scale = EEPROM_DATA[217];
            this.emissivity = (((uint)EEPROM_DATA[229] << 8) + EEPROM_DATA[228]) / 32768;

            for (int i = 0; i <= 63; i++)
            {
                this.a_ij[i] = EEPROM_DATA[i];
                if (this.a_ij[i] > 127)
                {
                    this.a_ij[i] = this.a_ij[i] - 256;
                }

                this.b_ij[i] = EEPROM_DATA[64 + i];
                if (this.b_ij[i] > 127)
                {
                    this.b_ij[i] = this.b_ij[i] - 256;
                }
            }

            // Calculates EEPROM alpha_ij constant values directly instead of using a spreadsheet. Taken from code from rmie.
            double da0_scale = System.Math.Pow(2, -EEPROM_DATA[0xe3]);
            double alpha_const =
                (double)
                (((uint)EEPROM_DATA[0xe1] << 8) + (uint)EEPROM_DATA[0xe0] * System.Math.Pow(2, -EEPROM_DATA[0xe2]));
            for (int i = 0; i <= 63; i++)
            {
                double alpha_var = (double)EEPROM_DATA[0x80 + i] * da0_scale;
                this.alpha_ij[i] = alpha_const + alpha_var;
            }
        }

        public void write_trimming_value(byte val)
        {
            const byte OFFSET = 0xAA;
            byte newAddress = (byte)(val - OFFSET);

            byte[] toWrite = { 0x04, newAddress, val, 0x56, 0x00 };
            this.i2c_write(toWrite, 0xC0, this.mlx90620_0xC0);
        }

        private static I2CDevice.I2CReadTransaction CreateReadTransaction(byte[] buffer, uint addr, byte addrSz)
        {
            I2CDevice.I2CReadTransaction readTransaction = I2CDevice.CreateReadTransaction(buffer);
            Type readTransactionType = typeof(I2CDevice.I2CReadTransaction);

            FieldInfo fieldInfo = readTransactionType.GetField(
                "Custom_InternalAddress", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(readTransaction, addr);

            fieldInfo = readTransactionType.GetField(
                "Custom_InternalAddressSize", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(readTransaction, addrSz);

            return readTransaction;
        }

        private static I2CDevice.I2CWriteTransaction CreateWriteTransaction(byte[] buffer, uint addr, byte addrSz)
        {
            I2CDevice.I2CWriteTransaction writeTransaction = I2CDevice.CreateWriteTransaction(buffer);
            Type writeTransactionType = typeof(I2CDevice.I2CWriteTransaction);

            FieldInfo fieldInfo = writeTransactionType.GetField(
                "Custom_InternalAddress", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(writeTransaction, addr);

            fieldInfo = writeTransactionType.GetField(
                "Custom_InternalAddressSize", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(writeTransaction, addrSz);

            return writeTransaction;
        }

        private void i2c_readAck(byte[] toRead, byte address, I2CDevice startAddress)
        {
            I2CDevice.I2CTransaction[] reading = new I2CDevice.I2CTransaction[]
                                                     {
                                                        CreateReadTransaction(toRead, address, 1) 
                                                     };
            int bytesRead = startAddress.Execute(reading, 100);
        }

        private void i2c_write(byte[] toWrite, byte address, I2CDevice startAddress)
        {
            I2CDevice.I2CTransaction[] reading = new I2CDevice.I2CTransaction[]
                                                     {
                                                        CreateWriteTransaction(toWrite, address, 1) 
                                                     };
            int bytesRead = startAddress.Execute(reading, 100);
        }
    }
}
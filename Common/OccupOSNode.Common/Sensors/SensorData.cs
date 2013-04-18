// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SensorData.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOS.CommonLibrary.Sensors
{
    using System;

    public struct Position
    {
        public float Depth;
        public int X;
        public int Y;
    }

    public class SensorData
    {
        public Sensor SensorType;
        public float AnalogLight;
        public int EntityCount;
        public Position[] EntityPositions;
        public float Humidity;
        public DateTime PollTime;
        public float Pressure;
        public DateTime ReadTime;
        public float Temperature;
        public float PowerWatt;
        public float SoundDb;
        public float VibrationHz;
        public float Windspeed;
    }

    public enum ConnectionStatus
    {
        Connected,
        Disconnected,
        Connecting,
        Disconnecting,
        Error,
        Unknown,
        Inapplicable
    }
}
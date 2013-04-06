// --------------------------------------------------------------------------------------------------------------------
// <copyright company="OccupOS" file="SensorData.cs">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   This is NOT supposed to reside in the OccupOSNode.Micro.Gadgeteer project. 
//   It is a temporary hack until Gadgeteer supports the .NET MF 4.3 Framework.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GadgeteerDemo
{
    public struct Position
    {
        public float Depth;
        public int X;
        public int Y;
    }

    public class SensorData
    {
        public float AnalogLight;
        public int EntityCount;
        public Position[] EntityPositions;
        public float Humidity;
        public float Pressure;
        public float Temperature;
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
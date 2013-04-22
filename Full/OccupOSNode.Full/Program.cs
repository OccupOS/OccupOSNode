// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("OccupOSNode.Tests")]

namespace OccupOSNode
{
    using System;
    using System.Threading;
    using OccupOS.CommonLibrary;
    using OccupOS.CommonLibrary.Sensors;
    using OccupOS.CommonLibrary.HardwareControllers;
    using OccupOSNode.NetworkControllers;
    using OccupOSNode.Sensors.Kinect;
    using System.Collections.Generic;

    internal class Program {
        private static void Main(string[] args) {

            var networkController = new FullEthernetNetworkController("192.168.0.2", 1333);
            FullNodeController nodeController = new FullNodeController(0, new HardwareController(), networkController);
            nodeController.Start(10, 5, 40);
        }
    }
}
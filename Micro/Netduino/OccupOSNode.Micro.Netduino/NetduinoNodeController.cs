// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArduinoNodeController.cs" company="OccupOS">
//   This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License along with this program. If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The arduino node controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOSNode.Micro
{
    using System;
    using System.IO;
    using OccupOS.CommonLibrary.NodeControllers;
    using OccupOS.CommonLibrary.NetworkControllers;
    using OccupOSNode.Micro.HardwareControllers.Netduino;

    internal class NetduinoNodeController : NodeController
    {
        public NetduinoNodeController(int ID, NetduinoHardwareController hardwareController, NetworkController networkController)
            : base(ID, hardwareController, networkController) { }
    }
}
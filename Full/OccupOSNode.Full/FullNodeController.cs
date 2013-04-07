// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FullNodeController.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("OccupOSNode.Tests")]

namespace OccupOSNode
{
    using OccupOS.CommonLibrary.HardwareControllers;
    using OccupOS.CommonLibrary.NetworkControllers;
    using OccupOS.CommonLibrary.NodeControllers;

    internal class FullNodeController : NodeController
    {
        public FullNodeController(int ID, HardwareController hardwareController, NetworkController networkController)
            : base(ID, hardwareController, networkController)
        {
        }
    }
}
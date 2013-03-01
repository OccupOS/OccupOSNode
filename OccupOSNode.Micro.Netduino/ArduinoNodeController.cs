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

    /// <summary>
    /// The arduino node controller.
    /// </summary>
    internal class ArduinoNodeController : NodeController
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="ArduinoNodeController"/> class.
        /// </summary>
        /// <exception cref="StorageDeviceMissingException">
        /// </exception>
        public ArduinoNodeController()
        {
            var rootDirectory = new DirectoryInfo(@"\SD\");
            if (rootDirectory.Exists)
            {
                this.LoadConfiguration();
            }
            else
            {
                throw new StorageDeviceMissingException("Couldn't find a connected SD card.");
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The poll sensors.
        /// </summary>
        public void PollSensors()
        {
            for (int k = 0; k < this.GetSensorCount(); k++)
            {
                this.AddSensorReading(this.GetSensor(k).GetData());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The load configuration.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        protected override void LoadConfiguration()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
﻿namespace OccupOSNode {
    using System;
    using System.IO;
    using OccupOS.CommonLibrary.NodeControllers;

    internal class FullNodeController : NodeController {

        public FullNodeController() { }

        public void PollSensors() {
        }

        protected override void LoadConfiguration() {
            throw new NotImplementedException();
        }
    }
}
using System;
using TFVXRP.Input;
using UnityEngine;

namespace TFVXRP.Scenery {
    internal interface IVehicleManager {
        internal Func<bool> GetVehicleIsReal { get; set; }
        internal Action<InputReceiver> SetReceiver { get; set; }
        internal Func<Vector3> GetStartingPadPosition { get; set; }
        internal OutFunc<bool, Vector3> GetHasLandingPadPositionAvailable { get; set; }

        internal void OnPrepareVehicle();
    }
}
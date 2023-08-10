using System.Collections.Generic;
using Support.Extensions;
using UnityEngine;
using UnityEngine.XR;

namespace Support.XR.Tracking.Valve.Vive.TrackedObjects {
    public partial class Controller : Support.XR.Tracking.TrackedObject {
        [SerializeField]
        private Role _controllerRole = Role.Disabled;

        protected sealed override InputDevice FindDevice() {
            var inputDevices = new List<InputDevice>();
            InputDevices.GetDevices(inputDevices);
            return inputDevices.Find(device => device.name.Equals($"{DeviceType.Controller.AsString()} ({_controllerRole.AsString()})"));
        }
    }
}
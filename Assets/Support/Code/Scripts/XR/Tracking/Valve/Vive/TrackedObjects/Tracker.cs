using System.Collections.Generic;
using Support.Extensions;
using UnityEngine;
using UnityEngine.XR;

namespace Support.XR.Tracking.Valve.Vive.TrackedObjects {
    public partial class Tracker : Support.XR.Tracking.TrackedObject {
        [SerializeField]
        private Role _trackerRole = Role.Disabled;

        protected sealed override InputDevice FindDevice() {
            var inputDevices = new List<InputDevice>();
            InputDevices.GetDevices(inputDevices);
            return inputDevices.Find(device => device.name.Equals($"{DeviceType.Tracker.AsString()} ({_trackerRole.AsString()})"));
        }

        protected new void SendImpulse(float amplitude, float duration) => throw EXCEPTION;
    }
}
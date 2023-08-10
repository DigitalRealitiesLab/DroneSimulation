using UnityEngine;

namespace Support.XR.Tracking.Valve.Vive.TrackedObjects {
    partial class Tracker {
        protected float BatteryValue {
            get {
                HasBatteryLevel(out float value);
                return value;
            }
        }

        protected new bool HasPrimary2DAxis(out Vector2 value) => throw EXCEPTION;

        protected new bool HasPrimary2DAxisClick(out bool value) => throw EXCEPTION;

        protected new bool HasPrimary2dAxisTouch(out bool value) => throw EXCEPTION;

        protected new bool HasSecondary2DAxis(out Vector2 value) => throw EXCEPTION;

        protected new bool HasSecondary2DAxisClick(out bool value) => throw EXCEPTION;

        protected new bool HasSecondary2DAxisTouch(out bool value) => throw EXCEPTION;

        protected new bool HasTrigger(out float value) => throw EXCEPTION;

        protected new bool HasTriggerButton(out bool value) => throw EXCEPTION;

        protected new bool HasGrip(out float value) => throw EXCEPTION;

        protected new bool HasGripButton(out bool value) => throw EXCEPTION;

        protected new bool HasPrimaryButton(out bool value) => throw EXCEPTION;

        protected new bool HasPrimaryTouch(out bool value) => throw EXCEPTION;

        protected new bool HasSecondaryButton(out bool value) => throw EXCEPTION;

        protected new bool HasSecondaryTouch(out bool value) => throw EXCEPTION;

        protected new bool HasMenuButton(out bool value) => throw EXCEPTION;

        protected new bool HasUserPresence(out bool value) => throw EXCEPTION;
    }
}
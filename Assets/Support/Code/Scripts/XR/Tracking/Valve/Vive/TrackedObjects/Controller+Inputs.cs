using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Global

namespace Support.XR.Tracking.Valve.Vive.TrackedObjects {
    partial class Controller {
        protected Vector2 TrackpadValue {
            get {
                HasPrimary2DAxis(out Vector2 value);
                return value;
            }
        }

        protected bool IsTrackpadPressed {
            get {
                HasPrimary2DAxisClick(out bool value);
                return value;
            }
        }

        protected bool IsTrackpadTouched {
            get {
                HasPrimary2dAxisTouch(out bool value);
                return value;
            }
        }

        protected float TriggerValue {
            get {
                HasTrigger(out float value);
                return value;
            }
        }

        protected bool IsTriggerPressed {
            get {
                HasTriggerButton(out bool value);
                return value;
            }
        }

        protected float GripValue {
            get {
                HasGrip(out float value);
                return value;
            }
        }

        protected bool IsGripPressed {
            get {
                HasGripButton(out bool value);
                return value;
            }
        }

        protected bool IsSandwichButtonPressed {
            get {
                HasPrimaryButton(out bool value);
                return value;
            }
        }

        protected float BatteryValue {
            get {
                HasBatteryLevel(out float value);
                return value;
            }
        }

        protected new bool HasSecondary2DAxis(out Vector2 value) => throw EXCEPTION;

        protected new bool HasSecondary2DAxisClick(out bool value) => throw EXCEPTION;

        protected new bool HasSecondary2DAxisTouch(out bool value) => throw EXCEPTION;

        protected new bool HasPrimaryTouch(out bool value) => throw EXCEPTION;

        protected new bool HasSecondaryButton(out bool value) => throw EXCEPTION;

        protected new bool HasSecondaryTouch(out bool value) => throw EXCEPTION;

        protected new bool HasMenuButton(out bool value) => throw EXCEPTION;

        protected new bool HasUserPresence(out bool value) => throw EXCEPTION;
    }
}
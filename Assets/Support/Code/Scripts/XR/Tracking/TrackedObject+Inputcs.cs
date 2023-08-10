using System.Data;
using UnityEngine;
using UnityEngine.XR;

namespace Support.XR.Tracking {
    partial class TrackedObject {
        protected static readonly DataException EXCEPTION = new();

        protected bool HasPrimary2DAxis(out Vector2 value) {
            if (_device.TryGetFeatureValue(CommonUsages.primary2DAxis, out value)) {
                return true;
            }

            value = Vector2.zero;
            return false;
        }

        protected bool HasTrigger(out float value) {
            if (_device.TryGetFeatureValue(CommonUsages.trigger, out value)) {
                return true;
            }

            value = 0f;
            return false;
        }

        protected bool HasGrip(out float value) {
            if (_device.TryGetFeatureValue(CommonUsages.grip, out value)) {
                return true;
            }

            value = 0f;
            return false;
        }

        protected bool HasSecondary2DAxis(out Vector2 value) {
            if (_device.TryGetFeatureValue(CommonUsages.secondary2DAxis, out value)) {
                return true;
            }

            value = Vector2.zero;
            return false;
        }

        protected bool HasSecondary2DAxisClick(out bool value) {
            if (_device.TryGetFeatureValue(CommonUsages.secondary2DAxisClick, out value)) {
                return true;
            }

            value = false;
            return false;
        }

        protected bool HasSecondary2DAxisTouch(out bool value) {
            if (_device.TryGetFeatureValue(CommonUsages.secondary2DAxisTouch, out value)) {
                return true;
            }

            value = false;
            return false;
        }

        protected bool HasPrimaryButton(out bool value) {
            if (_device.TryGetFeatureValue(CommonUsages.primaryButton, out value)) {
                return true;
            }

            value = false;
            return false;
        }

        protected bool HasPrimaryTouch(out bool value) {
            if (_device.TryGetFeatureValue(CommonUsages.primaryTouch, out value)) {
                return true;
            }

            value = false;
            return false;
        }

        protected bool HasSecondaryButton(out bool value) {
            if (_device.TryGetFeatureValue(CommonUsages.secondaryButton, out value)) {
                return true;
            }

            value = false;
            return false;
        }

        protected bool HasSecondaryTouch(out bool value) {
            if (_device.TryGetFeatureValue(CommonUsages.secondaryTouch, out value)) {
                return true;
            }

            value = false;
            return false;
        }

        protected bool HasGripButton(out bool value) {
            if (_device.TryGetFeatureValue(CommonUsages.gripButton, out value)) {
                return true;
            }

            value = false;
            return false;
        }

        protected bool HasTriggerButton(out bool value) {
            if (_device.TryGetFeatureValue(CommonUsages.triggerButton, out value)) {
                return true;
            }

            value = false;
            return false;
        }

        protected bool HasMenuButton(out bool value) {
            if (_device.TryGetFeatureValue(CommonUsages.menuButton, out value)) {
                return true;
            }

            value = false;
            return false;
        }

        protected bool HasPrimary2DAxisClick(out bool value) {
            if (_device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out value)) {
                return true;
            }

            value = false;
            return false;
        }

        protected bool HasPrimary2dAxisTouch(out bool value) {
            if (_device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out value)) {
                return true;
            }

            value = false;
            return false;
        }

        protected bool HasBatteryLevel(out float value) {
            if (_device.TryGetFeatureValue(CommonUsages.batteryLevel, out value)) {
                return true;
            }

            value = 0f;
            return false;
        }

        protected bool HasUserPresence(out bool value) {
            if (_device.TryGetFeatureValue(CommonUsages.userPresence, out value)) {
                return true;
            }

            value = false;
            return false;
        }
    }
}
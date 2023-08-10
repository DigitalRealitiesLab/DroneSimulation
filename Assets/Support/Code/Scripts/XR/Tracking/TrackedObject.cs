using System;
using Support.Extensions;
using UnityEngine;
using UnityEngine.XR;

// ReSharper disable MemberCanBePrivate.Global

namespace Support.XR.Tracking {
    public abstract partial class TrackedObject : MonoBehaviour {
        [SerializeField]
        private Vector3 _positionOffset, _rotationOffset;

        [SerializeField]
        private OffsetModifier _appliedOffsetModifier;

        private InputDevice _device;

        protected (Vector3 Position, Quaternion Rotation) InitialOffset => (_positionOffset, Quaternion.Euler(_rotationOffset));

        protected Vector3 Position {
            get {
                _device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 value);
                return value;
            }
        }

        protected Quaternion Rotation {
            get {
                _device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion value);
                return value;
            }
        }

        protected (Vector3 Position, Quaternion Rotation) Pose => (Position, Rotation);

        private void Start() => _device = FindDevice();

        private void Update() => OnUpdate();

        protected abstract InputDevice FindDevice();

        protected virtual void OnUpdate() {
            (Vector3 Position, Quaternion Rotation) pose = Pose;
            Quaternion rotation = pose.Rotation;
            Vector3 position = pose.Position;
            if (_appliedOffsetModifier.HasFlag(OffsetModifier.Rotation)) {
                rotation *= InitialOffset.Rotation;
            }

            if (_appliedOffsetModifier.HasFlag(OffsetModifier.Position)) {
                position += InitialOffset.Position.Rotate(_appliedOffsetModifier.HasFlag(OffsetModifier.AccumulatedRotationAndPosition) ? rotation : Quaternion.identity);
            }

            transform.SetPositionAndRotation(position, rotation);
        }

        protected void SendImpulse(float amplitude, float duration) {
            if (_device.TryGetHapticCapabilities(out HapticCapabilities capabilities) && capabilities.supportsImpulse) {
                _device.SendHapticImpulse(0, amplitude, duration);
            }
        }

        [Flags]
        private enum OffsetModifier {
            Rotation = 1,
            Position = 2,
            AccumulatedRotationAndPosition = 4
        }
    }
}
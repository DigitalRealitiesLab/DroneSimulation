using UnityEngine;

namespace DronePhysicsExtension {
    public class DroneRotor : MonoBehaviour {
        [SerializeField]
        private MeshRenderer _stationaryRotorBodyAndBlades, _rotatingRotorBody, _rotatingRotorBlades;

        private void Start() => SetIsRotating(false);

        private void SetIsRotating(bool isRotating) {
            _stationaryRotorBodyAndBlades.enabled = !isRotating;
            _rotatingRotorBody.enabled = isRotating;
            _rotatingRotorBlades.enabled = isRotating;
        }

        internal void UpdateRotation(Quaternion rotation, bool shouldRotate) {
            transform.localRotation *= rotation;
            SetIsRotating(shouldRotate);
        }
    }
}
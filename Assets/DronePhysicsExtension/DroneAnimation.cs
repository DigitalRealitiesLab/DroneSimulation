using UnityEngine;

namespace DronePhysicsExtension {
    public class DroneAnimation : MonoBehaviour {
        private const float _MINIMUM_ROTATION = 0f;
        private const float _MAXIMUM_ROTATION = 20000f;

        [Header("Propellers")]
        [SerializeField]
        internal DroneRotor FrontLeft;

        [SerializeField]
        internal DroneRotor FrontRight;
        
        [SerializeField]
        internal DroneRotor RearLeft;
        
        [SerializeField]
        internal DroneRotor RearRight;

        [SerializeField]
        private DronePhysics _dronePhysics;

        private float _breaksEngagedTimestamp = -1f;
        private bool BreaksAreEngaged => _breaksEngagedTimestamp > 0;

        private void Start() {
            if (!_dronePhysics) {
                _dronePhysics = GetComponent<DronePhysics>();
            }
        }

        private void Update() {
            _breaksEngagedTimestamp = BreaksAreEngaged switch {
                true when _dronePhysics.armed => -1f,
                false when !_dronePhysics.armed => Time.time,
                _ => _breaksEngagedTimestamp
            };
            float thrust = _dronePhysics.appliedForce.magnitude * 25f;

            FrontLeft.UpdateRotation(CalculateRotation(-_dronePhysics.appliedTorque.x - _dronePhysics.appliedTorque.z - _dronePhysics.appliedTorque.y, out bool motorFlIsRotating), motorFlIsRotating);
            FrontRight.UpdateRotation(CalculateRotation(-_dronePhysics.appliedTorque.x + _dronePhysics.appliedTorque.z + _dronePhysics.appliedTorque.y, out bool motorFrIsRotating), motorFrIsRotating);
            RearLeft.UpdateRotation(CalculateRotation(_dronePhysics.appliedTorque.x - _dronePhysics.appliedTorque.z + _dronePhysics.appliedTorque.y, out bool motorRlIsRotating), motorRlIsRotating);
            RearRight.UpdateRotation(CalculateRotation(_dronePhysics.appliedTorque.x + _dronePhysics.appliedTorque.z - _dronePhysics.appliedTorque.y, out bool motorRrIsRotating), motorRrIsRotating);

            #region Update - local functions
            Quaternion CalculateRotation(float torque, out bool isRotating) {
                float rotation = CalculateMotor() * 100f;
                isRotating = _dronePhysics.armed && rotation != 0;
                return Quaternion.Euler(0, isRotating ? rotation : rotation > 0f ? Mathf.Lerp(rotation, 0, (Time.time - _breaksEngagedTimestamp) / .75f) : 0, 0);
                float CalculateMotor() => thrust + Mathf.Clamp(torque * 100f, _MINIMUM_ROTATION, _MAXIMUM_ROTATION);
            }
            #endregion
        }
    }
}
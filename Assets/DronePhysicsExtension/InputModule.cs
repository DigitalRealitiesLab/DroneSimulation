using System;
using System.Collections;
using TFVXRP.DataCollection;
using TFVXRP.Input;
using UnityEngine;
using YueUltimateDronePhysics;
using Random = UnityEngine.Random;

namespace DronePhysicsExtension {
    [RequireComponent(typeof(DronePhysics))]
    internal class InputModule : InputReceiver {
        private const float _AVERAGE_STARTUP_TIME = 3.5f;

        private RaycastHit? _hit;

        private bool _isLanded = true;
        private bool _isLanding;
        private float _remainingStartupTime;
        private bool IsStarting => _remainingStartupTime > 0;
        private bool IsStarted => DistanceToGround > .15f;
        private Vector3 Velocity => Quaternion.Inverse(transform.rotation) * _rigidbody.velocity * -1;
        private float DistanceToGround => _hit?.distance ?? 0f;
        private Collider Bottom => _hit?.collider;

        #region Unity event functions
        private void Start() => GetComponentsRequiredForResetting();

        protected override void Update() {
            base.Update();
            MeasureDistanceToBottom();
            if (_isLanding && DistanceToGround < .25f) {
                TouchdownImminent?.Invoke();
            }

            #region Update - local functions
            void MeasureDistanceToBottom() {
                Ray downRay = new(transform.position, -Vector3.up);
                if (!Physics.Raycast(downRay, out RaycastHit hit)) {
                    _hit = null;
                    return;
                }

                _hit = hit;
            }
            #endregion
        }

        private void FixedUpdate() {
            if (_isResetting) {
                ResetDrone();
            }

            HandleTurbulences();
        }
        #endregion

        #region InputReceiver - Inheritance
        internal Action Startup;
        internal Action TouchdownImminent;

        internal override void OnStartup() {
            if (IsStarting || IsStarted || _isLanding) {
                return;
            }

            Startup?.Invoke();
            base.OnStartup();
            Transform currentTransform = transform;
            _lastStartPosition = currentTransform.position;
            _lastStartRotation = currentTransform.rotation;

            _isLanded = false;
            _remainingStartupTime = _AVERAGE_STARTUP_TIME * Random.Range(.75f, 1.25f);
        }

        protected override void ProcessInput() {
            thrust = (rawThrust + 1) * 0.5f;
            yaw = TransformInput(rawYaw, ratesConfig.proportionalGain, ratesConfig.exponentialGain);
            pitch = TransformInput(rawPitch, ratesConfig.proportionalGain, ratesConfig.exponentialGain);
            roll = TransformInput(rawRoll, ratesConfig.proportionalGain, ratesConfig.exponentialGain);

            float TransformInput(float rawInput, float p, float e) => rawInput * p + Mathf.Pow(rawInput, 2) * Mathf.Sign(rawInput) * e;
        }

        internal override void OnShutdown() {
            if (IsStarting || _isLanded || _isLanding) {
                return;
            }

            base.OnShutdown();
            _isLanding = !_isLanding;
        }
        #endregion

        #region Initial YueInputModule
        private const float _MINIMUM_FLIGHT_HEIGHT = .3f;

        
        public YueRatesConfiguration ratesConfig;

        [SerializeField]
        [Range(0f, 1f)]
        private float _pitchAndYawSpeedMultiplier = .75f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _yawSpeedMultiplier = .6f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _thrustSpeedMultiplier = .6f;

        [Tooltip("Raw Inputs should be between -1 to 1")]

        private float ReducedSteerAbilityMultiplierCloseToTheGround => Mathf.Clamp(1.5f * DistanceToGround + .25f, .25f, 1f);

        public float rawRoll {
            get {
                if (_isLanded) {
                    return 0f;
                }

                float r = input.Roll;
                if (r == 0f && Velocity.x != 0f) {
                    r = Mathf.Clamp(Velocity.x, -1, 1);
                }

                return -r * _pitchAndYawSpeedMultiplier * ReducedSteerAbilityMultiplierCloseToTheGround;
            }
        }

        public float rawPitch {
            get {
                if (_isLanded) {
                    return 0f;
                }

                float p = input.Pitch;
                if (p == 0f && Velocity.z != 0f) {
                    p = Mathf.Clamp(Velocity.z, -1, 1);
                }

                return p * _pitchAndYawSpeedMultiplier * ReducedSteerAbilityMultiplierCloseToTheGround;
            }
        }

        private float rawYaw {
            get {
                if (_isLanded) {
                    return 0f;
                }

                return input.Yaw * _yawSpeedMultiplier * ReducedSteerAbilityMultiplierCloseToTheGround;
            }
        }

        public float rawThrust {
            get {
                if (IsStarting) {
                    if (DistanceToGround > .1f) {
                        _remainingStartupTime -= Time.deltaTime;
                    }

                    return .5f;
                }

                if (_isLanding) {
                    return DistanceToGround < .25f ? -.25f : -.5f;
                }

                if (_isLanded) {
                    return 0f;
                }

                if (IsStarted && DistanceToGround < _MINIMUM_FLIGHT_HEIGHT && input.Thrust < 0) {
                    return 0f;
                }

                return input.Thrust * _thrustSpeedMultiplier;
            }
        }

        internal float thrust, yaw, pitch, roll;
        #endregion

        #region Resetting After Collision
        [SerializeField]
        [Tooltip("Time (in seconds) it takes until the reset animation starts.")]
        private float _collisionDisplayTime = 1.5f;

        [SerializeField]
        [Tooltip("Time (in seconds) it takes the vehicle to fully reset.")]
        private float _resetAnimationTime = 5f;

        private DronePhysics _physics;
        private DroneAnimation _animation;
        private Rigidbody _rigidbody;

        private bool _isResetting;
        private Vector3 _resetCollisionPosition;
        private Quaternion _resetCollisionRotation;
        private Vector3 _lastStartPosition;
        private Quaternion _lastStartRotation;
        private float _resetAnimationStartTime;
        private float TimeElapsedWhileResetting => Time.time - _resetAnimationStartTime;

        internal Action Landed;
        internal Action Collided;

        private void GetComponentsRequiredForResetting() {
            _physics = GetComponent<DronePhysics>();
            _animation = GetComponentInChildren<DroneAnimation>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void ResetDrone() {
            if (_resetAnimationStartTime == 0f) {
                _resetAnimationStartTime = Time.time;
            }

            float t = Mathf.Clamp(TimeElapsedWhileResetting / _resetAnimationTime, 0f, 1f);
            transform.position = Vector3.Lerp(_resetCollisionPosition, _lastStartPosition, t);
            transform.rotation = Quaternion.Lerp(_resetCollisionRotation, _lastStartRotation, t);

            if (CheckIfResetIsComplete()) {
                OnResetComplete();
            }

            #region ResetDrone - local functions
            bool CheckIfResetIsComplete() {
                Transform currentTransform = transform;
                return currentTransform.position == _lastStartPosition && currentTransform.rotation == _lastStartRotation && TimeElapsedWhileResetting > _resetAnimationTime;
            }

            void OnResetComplete() {
                _isResetting = false;
                _resetAnimationStartTime = 0f;
                Respawn.Invoke(_lastStartPosition, _lastStartRotation);
            }
            #endregion
        }

        private void OnCollisionEnter(Collision collision) {
            if (Bottom is not null && collision.collider.Equals(Bottom) && _isLanding) {
                _isLanding = false;
                _isLanded = true;
                Landed?.Invoke();
                SaveSnapshot.Invoke(_hit?.point ?? Vector3.zero);
            }
            else if (IsStarted) {
                OnCollided();
                StartCoroutine(WaitToStartTheResetAnimation());
            }

            #region OnCollisionEnter - local functions
            void OnCollided() {
                Collided?.Invoke();
                _physics.enabled = false;
                _animation.enabled = false;
                _rigidbody.isKinematic = true;
                Transform currentTransform = transform;
                _resetCollisionPosition = currentTransform.position;
                _resetCollisionRotation = currentTransform.rotation;
                DataCollectionManager.Log(DataCollectionManager.LogType.SaveCrashShot);
            }

            IEnumerator WaitToStartTheResetAnimation() {
                yield return new WaitForSeconds(_collisionDisplayTime);
                _isResetting = true;
            }
            #endregion
        }
        #endregion

        #region Turbulences
        [Header("Turbulences Configuration")]
        [SerializeField]
        private float _maximumTurbulenceHeight = .5f;

        [SerializeField]
        private float _minimumBreakBetweenTurbulences, _maximumBreakBetweenTurbulences;

        [SerializeField]
        private float _minimumTurbulenceDuration = .25f;

        [SerializeField]
        private float _maximumTurbulenceDuration = 1.25f;

        [SerializeField]
        [Tooltip("Controls a random multiplier to the application point of each turbulence force.\nCauses the Drone to move unsteadily, even hovering above a flat surface.")]
        [Range(0f, 1f)]
        private float _turbulenceAgitationStrength = .5f;

        [SerializeField]
        private bool _drawTurbulences;

        private float _timeSinceTurbulencesStarted, _timeSinceTurbulencesEnded, _timeUntilSwitchingToNextTurbulenceState;
        private Vector3 _flTurbulence, _frTurbulence, _rlTurbulence, _rrTurbulence;

        private void HandleTurbulences() {
            if (_timeSinceTurbulencesStarted > 0f) {
                if (_timeSinceTurbulencesStarted + _timeUntilSwitchingToNextTurbulenceState < Time.time) {
                    GenerateNewTurbulenceBreak();
                }
                else {
                    ApplyTurbulences();
                }
            }
            else if (_timeSinceTurbulencesEnded + _timeUntilSwitchingToNextTurbulenceState < Time.time) {
                GenerateNewTurbulences();
            }

            void GenerateNewTurbulenceBreak() {
                _timeSinceTurbulencesStarted = 0f;
                _timeSinceTurbulencesEnded = Time.time;
                _timeUntilSwitchingToNextTurbulenceState = Random.Range(_minimumBreakBetweenTurbulences, _maximumBreakBetweenTurbulences);
            }

            void ApplyTurbulences() {
                if (!(_maximumTurbulenceHeight > DistanceToGround) || IsStarting || !IsStarted || _isLanded || _isLanding || _isResetting) {
                    return;
                }

                _physics.ApplyTurbulence(_flTurbulence, _animation.FrontLeft.transform.position);
                _physics.ApplyTurbulence(_frTurbulence, _animation.FrontRight.transform.position);
                _physics.ApplyTurbulence(_rlTurbulence, _animation.RearLeft.transform.position);
                _physics.ApplyTurbulence(_rrTurbulence, _animation.RearRight.transform.position);

                if (!_drawTurbulences) {
                    return;
                }

                Debug.DrawRay(_animation.FrontLeft.transform.position, _flTurbulence * 5f, Color.red);
                Debug.DrawRay(_animation.FrontRight.transform.position, _frTurbulence * 5f, Color.green);
                Debug.DrawRay(_animation.RearLeft.transform.position, _rlTurbulence * 5f, Color.blue);
                Debug.DrawRay(_animation.RearRight.transform.position, _rrTurbulence * 5f, Color.black);
            }

            void GenerateNewTurbulences() {
                _flTurbulence = GenerateNewTurbulence(_animation.FrontLeft.transform);
                _frTurbulence = GenerateNewTurbulence(_animation.FrontRight.transform);
                _rlTurbulence = GenerateNewTurbulence(_animation.RearLeft.transform);
                _rrTurbulence = GenerateNewTurbulence(_animation.RearRight.transform);
                _timeSinceTurbulencesStarted = Time.time;
                _timeSinceTurbulencesEnded = 0f;
                _timeUntilSwitchingToNextTurbulenceState = Random.Range(_minimumTurbulenceDuration, _maximumTurbulenceDuration);

                Vector3 GenerateNewTurbulence(Transform rotor) {
                    Ray downRay = new(rotor.position, Vector3.down);
                    if (!Physics.Raycast(downRay, out RaycastHit downHit)) {
                        return Vector3.zero;
                    }

                    Ray upRay = new(downHit.point, downHit.normal);
                    if (!Physics.SphereCast(upRay, .0375f, out RaycastHit upHit) || upHit.distance > _maximumTurbulenceHeight) {
                        return Vector3.zero;
                    }

                    float distance = 1 - Mathf.Clamp01(upHit.distance / _maximumTurbulenceHeight);
                    return (new Vector3(
                        upHit.point.x * GenerateTurbulenceAgitationStrengthMultiplier(),
                        upHit.point.y * GenerateTurbulenceAgitationStrengthMultiplier(),
                        upHit.point.z * GenerateTurbulenceAgitationStrengthMultiplier()
                    ) - downHit.point) * distance;

                    float GenerateTurbulenceAgitationStrengthMultiplier() => Random.Range(1 - _turbulenceAgitationStrength, 1 + _turbulenceAgitationStrength);
                }
            }
        }
        #endregion
    }
}
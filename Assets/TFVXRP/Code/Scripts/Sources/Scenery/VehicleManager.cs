using System;
using Support.Constants;
using TFVXRP.DataCollection;
using TFVXRP.Input;
using UnityEngine;

namespace TFVXRP.Scenery {
    internal class VehicleManager : MonoBehaviour, IVehicleManager {
        [SerializeField]
        private InputReceiver _simulatedVehicle;

        [SerializeField]
        private InputReceiver _realVehicle;

        private readonly Vector3 _startPositionOffset = new(Constants.ZERO, .0275f, Constants.ZERO);
        private readonly Quaternion _startRotation = Quaternion.Euler(Constants.ZERO, Constants.ZERO, Constants.ZERO);

        private InputReceiver _simulatedVehicleInstance;

        private bool _vehicleIsPrepared;

        private IVehicleManager This => this;

        Func<bool> IVehicleManager.GetVehicleIsReal { get; set; }

        Action<InputReceiver> IVehicleManager.SetReceiver { get; set; }

        Func<Vector3> IVehicleManager.GetStartingPadPosition { get; set; }

        OutFunc<bool, Vector3> IVehicleManager.GetHasLandingPadPositionAvailable { get; set; }

        void IVehicleManager.OnPrepareVehicle() {
            if (_vehicleIsPrepared) {
                return;
            }

            _vehicleIsPrepared = true;
            DataCollectionManager.Log(DataCollectionManager.LogType.RoomFixated);
            if (This.GetVehicleIsReal.Invoke()) {
                _simulatedVehicle = null;
                This.SetReceiver.Invoke(_realVehicle);
            }
            else {
                InstantiateVehicle(This.GetStartingPadPosition.Invoke() + _startPositionOffset, _startRotation);
                Destroy(_realVehicle.gameObject);
            }
        }

        private void InstantiateVehicle(Vector3 position, Quaternion rotation) {
            if (_simulatedVehicleInstance != null) {
                _simulatedVehicleInstance.Respawn -= InstantiateVehicle;
                _simulatedVehicleInstance.SaveSnapshot -= OnSaveSnapshot;
                Destroy(_simulatedVehicleInstance.gameObject);
            }

            _simulatedVehicleInstance = Instantiate(_simulatedVehicle, position, rotation);
            _simulatedVehicleInstance.transform.SetParent(transform, true);
            _simulatedVehicleInstance.name = "SimulatedDrone";
            _simulatedVehicleInstance.Respawn += InstantiateVehicle;
            _simulatedVehicleInstance.SaveSnapshot += OnSaveSnapshot;
            This.SetReceiver.Invoke(_simulatedVehicleInstance);

            void OnSaveSnapshot(Vector3 dronePosition) {
                if (This.GetHasLandingPadPositionAvailable.Invoke(out Vector3 landingPadPosition)) {
                    DataCollectionManager.Log(DataCollectionManager.LogType.SaveVirtualSnapshot, dronePosition - landingPadPosition);
                }
                else {
                    DataCollectionManager.Log(DataCollectionManager.LogType.SaveSnapshot);
                }
            }
        }
    }
}
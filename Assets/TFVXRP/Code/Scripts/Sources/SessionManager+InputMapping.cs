using Support.Constants;
using Support.UserInput.InputTypes;
using TFVXRP.Input;

namespace TFVXRP {
    internal partial class SessionManager {
        private InputReceiver _receiver;

        private float _roll, _pitch, _yaw, _elevation;

        private InputVector Input => new(_roll, _pitch, _yaw, _elevation);

        private void OnSetReceiver(InputReceiver receiver) => _receiver = receiver;

        private void TransferInput() {
            if (_receiver) {
                _receiver.OnReceive(Input);
            }
        }

        private void MapInputs() {
            InputManager.AddListener(XBoxControllerInput.A, () => {
                if (_receiver) {
                    _receiver.OnStartup();
                }
            });
            InputManager.AddListener(XBoxControllerInput.B, () => {
                if (_receiver) {
                    _receiver.OnShutdown();
                }
            });
            InputManager.AddListener(XBoxControllerInput.Menu, VehicleManager.OnPrepareVehicle);
            InputManager.AddListener(XBoxControllerInput.LStick, vector2 => {
                _roll = vector2.x;
                _pitch = vector2.y;
            });
            InputManager.AddListener(XBoxControllerInput.RStick, vector2 => {
                _yaw = Constants.ZERO;
                _elevation = vector2.y;
            });
        }
    }
}
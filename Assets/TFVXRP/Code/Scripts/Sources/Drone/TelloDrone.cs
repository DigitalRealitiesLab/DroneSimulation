using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Support.Constants;
using Support.Extensions;
using TFVXRP.DataCollection;
using TFVXRP.Input;
using UnityEngine;

namespace TFVXRP.Drone {
    internal static class TelloDrone {
        private const string _IP_ADDRESS = "192.168.10.1";
        private const int _PORT = 8889;
        private const string _COMMAND_MESSAGE = "command";
        private const string _TAKEOFF_MESSAGE = "takeoff";
        private const string _LAND_MESSAGE = "land";
        private const string _BATTERY_MESSAGE = "battery?";
        private const float _PERCENTAGE_MULTIPLIER = 100f;

        private static UdpClient _commandClient;
        private static CancellationTokenSource _commandClientCancellationTokenSource;

        private static bool _hasZeroInput = true;
        private static float _timeSinceTheInputIsMaximumNegative;
        private static bool _attemptingToLand;
        private static SynchronizationContext _originalContext;

        internal static void EstablishConnection() {
            _commandClient.Connect(_IP_ADDRESS, _PORT);
            Send(_COMMAND_MESSAGE);
        }

        internal static void Initialize() {
            _originalContext = SynchronizationContext.Current;
            _commandClient = new UdpClient(_PORT);
            InitializeCommandClientListener();
            EstablishConnection();

            #region Initialize - local functions
            void InitializeCommandClientListener() {
                _commandClientCancellationTokenSource = new CancellationTokenSource();
                CancellationToken ct = _commandClientCancellationTokenSource.Token;

                Task.Run(() => {
                    while (!ct.IsCancellationRequested) {
                        try {
                            var endPoint = new IPEndPoint(IPAddress.Any, _PORT);
                            string message = Encoding.ASCII.GetString(_commandClient.Receive(ref endPoint));
                            Debug.Log(message);

                            if (!_attemptingToLand) {
                                continue;
                            }

                            _originalContext.Post(delegate { DataCollectionManager.Log(DataCollectionManager.LogType.SaveSnapshot); }, null);
                            _attemptingToLand = false;
                        }
                        catch (SocketException e) {
                            switch (e.ErrorCode) {
                                case 10004:
                                    // This is ok and will trigger if the thread is blocked waiting for a response while getting cancelled.
                                    break;
                                default:
                                    Debug.LogError(e.ToString());
                                    break;
                            }
                        }
                        catch (Exception e) {
                            Debug.LogError(e.ToString());
                        }
                    }

                    ct.ThrowIfCancellationRequested();
                }, ct);
            }
            #endregion
        }

        private static void Send(string message) {
            byte[] sendBytes = Encoding.ASCII.GetBytes(message);
            _commandClient.Send(sendBytes, sendBytes.Length);
        }

        internal static void Takeoff() {
            _attemptingToLand = false;
            Send(_TAKEOFF_MESSAGE);
        }

        internal static void Fly(InputVector input) {
            if (input.IsZero) {
                if (_hasZeroInput) {
                    return;
                }

                _hasZeroInput = true;
            }
            else {
                _hasZeroInput = false;
            }

            float thrust = CalculateThrust();
            Send($"rc {input.Roll * _PERCENTAGE_MULTIPLIER} {input.Pitch * _PERCENTAGE_MULTIPLIER} {thrust * _PERCENTAGE_MULTIPLIER} {input.Yaw * _PERCENTAGE_MULTIPLIER}");

            #region Fly - local functions
            float CalculateThrust() {
                float calculatedThrust = input.Thrust;
                if (input.Thrust > -.9f) {
                    _timeSinceTheInputIsMaximumNegative = Constants.ZERO;
                }
                else {
                    if (_timeSinceTheInputIsMaximumNegative.IsNotEqualTo(Constants.ZERO)) {
                        float difference = Time.time - _timeSinceTheInputIsMaximumNegative;
                        if (!(difference > .5f)) {
                            return calculatedThrust;
                        }

                        calculatedThrust = Constants.ZERO;
                        if (difference > .55f) {
                            _timeSinceTheInputIsMaximumNegative = Constants.ZERO;
                        }
                    }
                    else {
                        _timeSinceTheInputIsMaximumNegative = Time.time;
                    }
                }

                return calculatedThrust;
            }
            #endregion
        }

        internal static void Land() {
            if (_attemptingToLand) {
                return;
            }

            _attemptingToLand = true;
            Send(_LAND_MESSAGE);
        }

        internal static void BatteryLevel() => Send(_BATTERY_MESSAGE);

        internal static void ResetLandingAttempt() => _attemptingToLand = false;

        internal static void DeInitialize() {
            Land();
            _commandClientCancellationTokenSource.Cancel();
            _commandClient.Close();
        }
    }
}
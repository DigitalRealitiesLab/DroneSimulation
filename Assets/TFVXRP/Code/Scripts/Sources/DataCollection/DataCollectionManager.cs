using System;
using Support.Extensions;
using TFVXRP.DataCollection.Savegames;
using TFVXRP.Scenery;
using UnityEngine;

namespace TFVXRP.DataCollection {
    internal static class DataCollectionManager {
        private static bool _collectData;
        private static DateTime? _startTime;

        private static ConditionSavegame _conditionSavegame;
        private static SessionSavegame _sessionSavegame;
        private static AccumulativeSavegame _accumulativeSavegame;

        internal static void InitializeDataCollecting(bool shouldCollectData, string userID, SceneType sceneType, StudyTaskType taskType) {
            _collectData = shouldCollectData;
            if (!_collectData) {
                return;
            }

            _startTime = null;
            _conditionSavegame = ConditionSavegame.Initialize(userID, sceneType, taskType);
            _sessionSavegame = SessionSavegame.Initialize(userID, sceneType, taskType);
            _sessionSavegame.AddEntry(DateTime.Now, StringFormatter.SessionStart(sceneType.AsString(), taskType.AsString()));
            _sessionSavegame.Save();
            _accumulativeSavegame = AccumulativeSavegame.Initialize();
            _accumulativeSavegame.Save();
        }

        internal static void Log(LogType logType, Vector3 distanceToTargetVector = default) {
            if (!_collectData) {
                return;
            }

            DateTime timestamp = DateTime.Now;
            _sessionSavegame.AddEntry(DateTime.Now, logType.AsString().ToLogEntry(_conditionSavegame.FlightNumber));
            _sessionSavegame.Save();

            switch (logType) {
                case LogType.StartingVehicle:
                    _startTime ??= timestamp;
                    break;
                case LogType.RoomFixated:
                case LogType.StoppingVehicle:
                    break;
                case LogType.SaveSnapshot:
                case LogType.SaveVirtualSnapshot:
                    if (_startTime is null) {
                        break;
                    }

                    const int meterToCentimeterMultiplier = 100;
                    _conditionSavegame.AddFlight(
                        _startTime ?? DateTime.Now,
                        DateTime.Now,
                        logType.Equals(LogType.SaveVirtualSnapshot) ? distanceToTargetVector.magnitude * meterToCentimeterMultiplier : -1f,
                        true,
                        logType.Equals(LogType.SaveVirtualSnapshot) ? CalculateLandingPosition(distanceToTargetVector) : CompassDirection.Invalid
                    );
                    _conditionSavegame.Save();
                    _startTime = null;
                    break;
                case LogType.SaveCrashShot:
                    if (_startTime is null) {
                        break;
                    }

                    _conditionSavegame.AddFlight(_startTime ?? DateTime.Now, DateTime.Now, -1, false, CompassDirection.Invalid);
                    _conditionSavegame.Save();
                    _startTime = null;
                    break;
                case LogType.CalculateResults:
                    _conditionSavegame.Save();
                    _sessionSavegame.Save();
                    _accumulativeSavegame.AddCondition(_conditionSavegame.ToCondition());
                    _accumulativeSavegame.Save();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }

            CompassDirection CalculateLandingPosition(Vector3 vector3) {
                float angle = Mathf.Atan2(vector3.z, vector3.x);
                int octant = Mathf.RoundToInt(8 * angle / (2 * Mathf.PI) + 8) % 8;
                return (CompassDirection)octant;
            }
        }

        internal static void SaveManualCollectedAccuracies((int Accuracy, CompassDirection LandingDirection)[] accuraciesAndLandingDirections) {
            _conditionSavegame.AddAccuraciesAndLandingDirections(accuraciesAndLandingDirections);
            _conditionSavegame.Save();
            _accumulativeSavegame ??= AccumulativeSavegame.Initialize();
            _accumulativeSavegame.UpdateCondition(_conditionSavegame.ToCondition());
            _accumulativeSavegame.Save();
        }

        internal static void ExportToCsv() {
            _accumulativeSavegame ??= AccumulativeSavegame.Initialize();
            _accumulativeSavegame.ExportSavegameToCsv();
        }

        internal enum LogType {
            [EnumExtensions.StringRepresentation("Room Fixated.")]
            RoomFixated,

            [EnumExtensions.StringRepresentation("Starting the Vehicle.")]
            StartingVehicle,

            [EnumExtensions.StringRepresentation("Stopping the Vehicle.")]
            StoppingVehicle,

            [EnumExtensions.StringRepresentation("Snapshot saved.")]
            SaveSnapshot,

            [EnumExtensions.StringRepresentation("Snapshot saved.")]
            SaveVirtualSnapshot,

            [EnumExtensions.StringRepresentation("Snapshot saved.")]
            SaveCrashShot,

            [EnumExtensions.StringRepresentation("Calculate Results.")]
            CalculateResults
        }
    }
}
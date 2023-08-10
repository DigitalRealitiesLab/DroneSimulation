using System;
using System.Collections.Generic;
using TFVXRP.Scenery;
using UnityEngine;

namespace TFVXRP.DataCollection.Savegames {
    [Serializable]
    internal class ConditionSavegame : IterativeSavegame {
        [SerializeField]
        private List<Flight> _flights;

        [SerializeField]
        private SceneType _sceneType;

        [SerializeField]
        private StudyTaskType _taskType;

        internal string FlightNumber => _flights.Count.ToString();

        internal static ConditionSavegame Initialize(string id, SceneType sceneType, StudyTaskType taskType) {
            if (Load(out ConditionSavegame loadedSavegame, id.ToConditionName(sceneType, taskType), id)) {
                return loadedSavegame;
            }

            var savegame = Initialize<ConditionSavegame>(id);
            savegame._flights = new List<Flight>();
            savegame._sceneType = sceneType;
            savegame._taskType = taskType;
            return savegame;
        }

        internal void Save() => Save(_id.ToConditionName(_sceneType, _taskType), _id);

        internal void AddFlight(DateTime start, DateTime end, float accuracy, bool withoutCrash, CompassDirection landingDirection) =>
            _flights.Add(new Flight(start, end, accuracy, withoutCrash, landingDirection));

        internal void AddAccuraciesAndLandingDirections((int Accuracy, CompassDirection LandingDirection)[] accuraciesAndLandingDirections) {
            var crashedFlightsCounter = 0;
            for (var i = 0; i < accuraciesAndLandingDirections.Length; i++) {
                if (_flights[i + crashedFlightsCounter].WithoutCrash) {
                    _flights[i + crashedFlightsCounter] = _flights[i + crashedFlightsCounter]
                        .AddAccuracyAndLandingDirection(accuraciesAndLandingDirections[i].Accuracy, accuraciesAndLandingDirections[i].LandingDirection);
                }
                else {
                    crashedFlightsCounter++;
                    i--;
                }
            }
        }

        internal AccumulativeSavegame.Condition ToCondition() => new(_id, _sceneType, _taskType, _flights);
    }
}
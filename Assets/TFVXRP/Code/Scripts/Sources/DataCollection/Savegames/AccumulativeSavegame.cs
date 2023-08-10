using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TFVXRP.Scenery;
using UnityEngine;

namespace TFVXRP.DataCollection.Savegames {
    [Serializable]
    internal class AccumulativeSavegame : Savegame {
        private const string _FILENAME = "AccumulatedData";
        private const string _CSV_FILENAME = "data.csv";

        [SerializeField]
        private List<Condition> _conditions;

        internal static AccumulativeSavegame Initialize() {
            if (Load(out AccumulativeSavegame loadedSavegame, _FILENAME)) {
                return loadedSavegame;
            }

            var savegame = new AccumulativeSavegame {
                _conditions = new List<Condition>()
            };
            return savegame;
        }

        internal void AddCondition(Condition condition) => _conditions.Add(condition);

        internal void UpdateCondition(Condition condition) {
            int index = _conditions.FindIndex(c => c.Equals(condition));
            if (index > -1) {
                _conditions[index] = condition;
            }
        }

        internal void ExportSavegameToCsv() {
            string stringToWrite = _conditions.Aggregate(
                "id;sceneType;taskType;targetNumber;flightNumber;flightDuration;cmAccuracy;ringAccuracy;withoutCrash;targetMissed;overshotDirection\n",
                (currentStringToWrite, condition) => condition.Flights.Aggregate(
                    currentStringToWrite, (current, flight) => current +
                                                               $"{condition.ID};" +
                                                               $"{condition.SceneType.ToString()};" +
                                                               $"{condition.TaskType.ToString()};" +
                                                               $"{CalculateTargetNumber(condition.Flights, flight)};" +
                                                               $"{CalculateFlightNumber(condition.Flights, flight)};" +
                                                               $"{flight.FlightDurationInS};" +
                                                               $"{flight.AccuracyInCm};" +
                                                               $"{flight.AccuracyInRings};" +
                                                               $"{flight.WithoutCrash};" +
                                                               $"{flight.TargetMissed};" +
                                                               $"{flight.LandingDirection};" +
                                                               "\n"
                )
            );
            var writer = new StreamWriter(ToPath(defaultFolderPath, _CSV_FILENAME), false);
            writer.Write(stringToWrite);
            writer.Close();

            int CalculateTargetNumber(List<Flight> flights, Flight currentFlight) {
                var currentTargetNumber = 1;
                for (var i = 0; i < flights.FindIndex(flight => flight.Equals(currentFlight)); i++) {
                    if (flights[i].WithoutCrash) {
                        currentTargetNumber++;
                    }
                }

                return currentTargetNumber;
            }

            int CalculateFlightNumber(List<Flight> flights, Flight currentFlight) {
                int currentFlightIndex = flights.FindIndex(flight => flight.Equals(currentFlight));
                var currentFlightNumber = 0;
                do {
                    currentFlightNumber++;
                    currentFlightIndex--;
                } while (currentFlightIndex > 0 && !flights[currentFlightIndex].WithoutCrash);

                return currentFlightNumber;
            }
        }

        internal void Save() => Save(_FILENAME);

        [Serializable]
        internal struct Condition : IEquatable<Condition> {
            [SerializeField]
            [HideInInspector]
            private string _id;

            [SerializeField]
            [HideInInspector]
            private SceneType _sceneType;

            [SerializeField]
            [HideInInspector]
            private StudyTaskType _taskType;

            [SerializeField]
            [HideInInspector]
            private List<Flight> _flights;

            internal Condition(string id, SceneType sceneType, StudyTaskType taskType, List<Flight> flights) {
                _id = id;
                _sceneType = sceneType;
                _taskType = taskType;
                _flights = flights;
            }

            internal readonly string ID => _id;
            internal readonly SceneType SceneType => _sceneType;
            internal readonly StudyTaskType TaskType => _taskType;
            internal readonly List<Flight> Flights => _flights;

            public bool Equals(Condition other) => _id == other._id && _sceneType == other._sceneType && _taskType == other._taskType;

            public override bool Equals(object obj) => obj is Condition other && Equals(other);

            public override int GetHashCode() => HashCode.Combine(_id, (int)_sceneType, (int)_taskType);
        }
    }
}
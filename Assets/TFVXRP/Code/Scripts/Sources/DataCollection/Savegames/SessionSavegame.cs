using System;
using System.Collections.Generic;
using System.Linq;
using Support.Extensions;
using TFVXRP.Scenery;
using UnityEngine;

namespace TFVXRP.DataCollection.Savegames {
    [Serializable]
    internal class SessionSavegame : IterativeSavegame {
        [SerializeField]
        private List<Condition> _conditions;

        private Condition CurrentCondition => _conditions.Last();

        internal IEnumerable<(string Description, IEnumerable<(string Timestamp, string Description)> Entries)> Conditions =>
            _conditions.Select(condition => (_id.ToConditionName(condition.SceneType, condition.TaskType), condition.Entries));

        internal static SessionSavegame Initialize(string id, SceneType sceneType, StudyTaskType taskType) {
            SessionSavegame savegame;
            if (Load(out SessionSavegame loadedSavegame, id.ToSessionName(), id)) {
                savegame = loadedSavegame;
                savegame._conditions.Add(new Condition(sceneType, taskType));
            }
            else {
                savegame = Initialize<SessionSavegame>(id);
                savegame._conditions = new List<Condition> { new(sceneType, taskType) };
            }

            return savegame;
        }

        internal void Save() => Save(_id.ToSessionName(), _id);

        internal void AddEntry(DateTime timestamp, string description) => CurrentCondition.AddEntry(timestamp, description);

        [Serializable]
        private struct Condition {
            [SerializeField]
            private SceneType _sceneType;

            [SerializeField]
            private StudyTaskType _taskType;

            [SerializeField]
            private List<ConditionEntry> _entries;

            internal Condition(SceneType sceneType, StudyTaskType taskType) {
                _sceneType = sceneType;
                _taskType = taskType;
                _entries = new List<ConditionEntry>();
            }

            internal string SceneType => _sceneType.AsString();
            internal string TaskType => _taskType.AsString();
            internal IEnumerable<(string Timestamp, string Description)> Entries => _entries.Select(entry => (entry.Timestamp, entry.Description));

            internal void AddEntry(DateTime timestamp, string description) => _entries.Add(new ConditionEntry(timestamp, description));

            [Serializable]
            private struct ConditionEntry {
                [SerializeField]
                private long _timestamp;

                [SerializeField]
                private string _description;

                internal ConditionEntry(DateTime timestamp, string description) {
                    _timestamp = timestamp.Ticks;
                    _description = description;
                }

                internal string Timestamp => new DateTime(_timestamp).ToLongTimeString();
                internal string Description => _description;
            }
        }
    }
}
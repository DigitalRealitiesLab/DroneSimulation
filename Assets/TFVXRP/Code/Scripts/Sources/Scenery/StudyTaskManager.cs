using System;
using UnityEngine;

namespace TFVXRP.Scenery {
    internal class StudyTaskManager : MonoBehaviour, IStudyTaskManager {
        [SerializeField]
        private StudyTask[] _tasks;

        private StudyTask _currentTask;

        private IStudyTaskManager This => this;

        private void Start() {
            StudyTaskType taskType = This.GetTaskType.Invoke();
            foreach (StudyTask task in _tasks) {
                if (task.Type.Equals(taskType)) {
                    task.gameObject.SetActive(true);
                    _currentTask = task;
                }
                else {
                    Destroy(task.gameObject);
                }
            }
        }

        Func<StudyTaskType> IStudyTaskManager.GetTaskType { get; set; }

        Vector3 IStudyTaskManager.OnGetStartingPadPosition() => _currentTask.StartingPadPosition;

        bool IStudyTaskManager.OnHasLandingPadPositionAvailable(out Vector3 landingPadPosition) => _currentTask.HasLandingPadPositionAvailable(out landingPadPosition);
    }
}
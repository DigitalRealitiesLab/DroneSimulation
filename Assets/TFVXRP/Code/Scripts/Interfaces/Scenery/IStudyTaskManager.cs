using System;
using UnityEngine;

namespace TFVXRP.Scenery {
    internal interface IStudyTaskManager {
        internal Func<StudyTaskType> GetTaskType { get; set; }

        internal Vector3 OnGetStartingPadPosition();

        internal bool OnHasLandingPadPositionAvailable(out Vector3 landingPadPosition);
    }
}
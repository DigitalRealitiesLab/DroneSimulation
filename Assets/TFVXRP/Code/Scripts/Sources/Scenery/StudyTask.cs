using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TFVXRP.Scenery {
    internal class StudyTask : MonoBehaviour {
        [SerializeField]
        private StudyTaskType _type = StudyTaskType.Obsolete;

        [SerializeField]
        private Transform _startingPad;

        [SerializeField]
        private List<Transform> _taskFlightOrder;

        internal StudyTaskType Type => _type;
        internal Vector3 StartingPadPosition => _startingPad.position;

        internal bool HasLandingPadPositionAvailable(out Vector3 landingPadPosition) {
            if (_taskFlightOrder.Count > 0) {
                Transform nextLandingPad = _taskFlightOrder.First();
                landingPadPosition = nextLandingPad.position;
                return _taskFlightOrder.Remove(nextLandingPad);
            }

            landingPadPosition = Vector3.zero;
            return false;
        }
    }
}
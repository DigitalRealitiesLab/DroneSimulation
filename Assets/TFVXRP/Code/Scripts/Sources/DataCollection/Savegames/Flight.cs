using System;
using Support.Constants;
using Support.Extensions;
using UnityEngine;

namespace TFVXRP.DataCollection.Savegames {
    [Serializable]
    internal struct Flight {
        [SerializeField]
        [HideInInspector]
        private long _end;

        [SerializeField]
        [HideInInspector]
        private long _start;

        [SerializeField]
        [HideInInspector]
        private float _accuracy;

        [SerializeField]
        [HideInInspector]
        private bool _withoutCrash;

        [SerializeField]
        [HideInInspector]
        private CompassDirection _landingDirection;

        internal Flight(DateTime start, DateTime end, float accuracy, bool withoutCrash, CompassDirection landingDirection) {
            _start = start.Ticks;
            _end = end.Ticks;
            _accuracy = accuracy;
            _withoutCrash = withoutCrash;
            _landingDirection = landingDirection;
        }

        private DateTime Start => new(_start);
        private DateTime End => new(_end);

        private TimeSpan FlightDuration => End - Start;
        internal double FlightDurationInS => FlightDuration.TotalSeconds;
        internal double FlightDurationInMs => FlightDuration.TotalMilliseconds;

        internal bool HasAccuracy => _accuracy >= Constants.ZERO;

        internal int AccuracyInCm => Mathf.RoundToInt(_accuracy);

        internal int AccuracyInRings => _accuracy switch {
            <= 2f => 0,
            <= 4f => 1,
            <= 6f => 2,
            <= 8f => 3,
            <= 10f => 4,
            <= 12f => 5,
            <= 14f => 6,
            <= 16f => 7,
            <= 18f => 8,
            <= 20f => 9,
            <= 22f => 10,
            <= 24f => 11,
            _ => 12
        };

        internal bool WithoutCrash => _withoutCrash;
        internal bool TargetMissed => AccuracyInRings >= 12;

        internal string LandingDirection => _landingDirection.AsString();

        internal Flight AddAccuracyAndLandingDirection(float accuracy, CompassDirection landingDirection) => new(Start, End, accuracy, true, landingDirection);

        public override string ToString() => StringFormatter.AddVerticalSlashDividersBetweenStrings(
            Start.ToLongTimeString(),
            End.ToLongTimeString(),
            FlightDuration.ToMinutesSecondsAndMilliseconds(),
            FlightDurationInS,
            FlightDurationInMs,
            AccuracyInCm,
            AccuracyInRings,
            WithoutCrash ? "No Crash" : "Crash",
            TargetMissed ? "Target Missed" : "On Target",
            LandingDirection
        );
    }
}
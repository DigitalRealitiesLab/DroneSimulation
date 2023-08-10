using System;
using Support.Extensions;
using TFVXRP.Scenery;

namespace TFVXRP.DataCollection {
    internal static class StringFormatter {
        internal static string SessionStart(string sceneType, string taskType) => $"Started {sceneType}-Session with {taskType}-Task.";

        internal static string ToLogEntry(this string logText, string flightNumber) => $" ({flightNumber}): {logText}";

        internal static string ToConditionName(this string id, SceneType sceneType, StudyTaskType taskType) => id.ToConditionName(sceneType.AsString(), taskType.AsString());

        internal static string ToConditionName(this string id, string sceneType, string taskType) => $"{id}_{sceneType}_{taskType}";

        internal static string ToSessionName(this string id) => id + "_session";

        internal static string AddVerticalSlashDividersBetweenStrings(params IConvertible[] contents) {
            if (contents.IsEmpty()) {
                return string.Empty;
            }

            var s = $"{contents[0]}";
            var counter = 1;
            while (counter < contents.Length) {
                s += "   |   " + contents[counter++];
            }

            return s;
        }

        internal static string ToMinutesSecondsAndMilliseconds(this TimeSpan timeSpan) {
            string minutes = LeadingZero(timeSpan.Minutes);
            string seconds = LeadingZero(timeSpan.Seconds);
            string milliSeconds = LeadingTwofoldZero(timeSpan.Milliseconds);
            return $"{minutes}:{seconds},{milliSeconds}";

            string LeadingZero(int number) => number < 10 ? $"0{number}" : $"{number}";
            string LeadingTwofoldZero(int number) => number < 100 ? $"0{LeadingZero(number)}" : $"{number}";
        }
    }
}
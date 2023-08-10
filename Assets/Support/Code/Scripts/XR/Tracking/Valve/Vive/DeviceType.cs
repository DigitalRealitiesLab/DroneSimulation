using Support.Extensions;

namespace Support.XR.Tracking.Valve.Vive {
    internal enum DeviceType {
        [EnumExtensions.StringRepresentation("SteamVR Controller (Vive Wand)")]
        Controller,

        [EnumExtensions.StringRepresentation("SteamVR Tracker")]
        Tracker
    }
}
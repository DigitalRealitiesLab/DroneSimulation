using Support.Extensions;

namespace Support.XR.Tracking.Valve.Vive {
    internal enum Role {
        [EnumExtensions.StringRepresentation("Disabled")]
        Disabled,

        [EnumExtensions.StringRepresentation("Left Hand")]
        LeftHand,

        [EnumExtensions.StringRepresentation("Right Hand")]
        RightHand,

        [EnumExtensions.StringRepresentation("Left Foot")]
        LeftFoot,

        [EnumExtensions.StringRepresentation("Right Foot")]
        RightFoot,

        [EnumExtensions.StringRepresentation("Left Shoulder")]
        LeftShoulder,

        [EnumExtensions.StringRepresentation("Right Shoulder")]
        RightShoulder,

        [EnumExtensions.StringRepresentation("Left Elbow")]
        LeftElbow,

        [EnumExtensions.StringRepresentation("Right Elbow")]
        RightElbow,

        [EnumExtensions.StringRepresentation("Left Knee")]
        LeftKnee,

        [EnumExtensions.StringRepresentation("Right Knee")]
        RightKnee,

        [EnumExtensions.StringRepresentation("Waist")]
        Waist,

        [EnumExtensions.StringRepresentation("Chest")]
        Chest,

        [EnumExtensions.StringRepresentation("Camera")]
        Camera,

        [EnumExtensions.StringRepresentation("Keyboard")]
        Keyboard
    }
}
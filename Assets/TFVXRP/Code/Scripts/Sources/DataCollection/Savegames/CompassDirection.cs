using Support.Extensions;

namespace TFVXRP.DataCollection.Savegames {
    internal enum CompassDirection : short {
        [EnumExtensions.StringRepresentation("Invalid")]
        Invalid = 8,

        [EnumExtensions.StringRepresentation("E")]
        East = 0,

        [EnumExtensions.StringRepresentation("NE")]
        NorthEast = 1,

        [EnumExtensions.StringRepresentation("N")]
        North = 2,

        [EnumExtensions.StringRepresentation("NW")]
        NorthWest = 3,

        [EnumExtensions.StringRepresentation("W")]
        West = 4,

        [EnumExtensions.StringRepresentation("SW")]
        SouthWest = 5,

        [EnumExtensions.StringRepresentation("S")]
        South = 6,

        [EnumExtensions.StringRepresentation("SE")]
        SouthEast = 7
    }
}
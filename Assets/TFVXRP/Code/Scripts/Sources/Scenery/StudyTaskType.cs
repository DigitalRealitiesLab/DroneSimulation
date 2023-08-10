using Support.Extensions;

namespace TFVXRP.Scenery {
    internal enum StudyTaskType {
        [EnumExtensions.StringRepresentation("Obsolete")]
        Obsolete,

        [EnumExtensions.StringRepresentation("Empty")]
        Empty,

        [EnumExtensions.StringRepresentation("FittsTest")]
        FittsTest,

        [EnumExtensions.StringRepresentation("FittsTestWithObstacles")]
        FittsTestWithObstacles
    }
}
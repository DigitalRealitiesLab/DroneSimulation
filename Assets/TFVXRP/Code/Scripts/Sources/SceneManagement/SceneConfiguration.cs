using TFVXRP.Scenery;

namespace TFVXRP.SceneManagement {
    internal static class SceneConfiguration {
        internal static SceneType SceneType = SceneType.VR;
        internal static StudyTaskType TaskType = StudyTaskType.FittsTestWithObstacles;
        internal static string UserID = "-1";

        internal static (bool SceneryIsReal, bool VehicleIsReal) GetSceneryRealism => (SceneType != SceneType.VR, SceneType is SceneType.Reality or SceneType.RealityWithoutHmd);
    }
}
using Support.UserInput;
using TFVXRP.DataCollection;
using TFVXRP.SceneManagement;
using TFVXRP.Scenery;
using UnityEngine;

namespace TFVXRP {
    internal sealed partial class SessionManager : MonoBehaviour {
        [Header("Connection Sources")]
        [SerializeField]
        private GameObject _studyTaskManagerSource;

        [SerializeField]
        private GameObject _roomSource;

        [SerializeField]
        private GameObject _vehicleManagerSource;

        [SerializeField]
        private GameObject _inputManagerSource;

        [Header("Data Collection Management")]
        [SerializeField]
        private bool _collectData = true;

        private IStudyTaskManager StudyTaskManager => _studyTaskManagerSource.GetComponent<IStudyTaskManager>();
        private IRoom Room => _roomSource.GetComponent<IRoom>();
        private IVehicleManager VehicleManager => _vehicleManagerSource.GetComponent<IVehicleManager>();
        private IInputManager InputManager => _inputManagerSource.GetComponent<IInputManager>();

        private void Awake() {
            StudyTaskManager.GetTaskType += () => SceneConfiguration.TaskType;
            Room.GetSceneryRealism += () => SceneConfiguration.GetSceneryRealism;
            VehicleManager.GetVehicleIsReal += () => SceneConfiguration.GetSceneryRealism.VehicleIsReal;
            VehicleManager.SetReceiver += OnSetReceiver;
            VehicleManager.GetStartingPadPosition += StudyTaskManager.OnGetStartingPadPosition;
            VehicleManager.GetHasLandingPadPositionAvailable += StudyTaskManager.OnHasLandingPadPositionAvailable;
            MapInputs();
            DataCollectionManager.InitializeDataCollecting(_collectData, SceneConfiguration.UserID, SceneConfiguration.SceneType, SceneConfiguration.TaskType);
        }

        private void Update() => TransferInput();
    }
}
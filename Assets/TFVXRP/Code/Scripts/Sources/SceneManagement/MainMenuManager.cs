using Support.Extensions;
using TFVXRP.DataCollection;
using TFVXRP.Scenery;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TFVXRP.SceneManagement {
    internal class MainMenuManager : MonoBehaviour {
        [SerializeField]
        private Button _quitGameButton;

        [SerializeField]
        private TMP_InputField _userIDInputField;

        [SerializeField]
        private TMP_Dropdown _sceneTypeDropdown, _taskTypeDropdown;

        [SerializeField]
        private Button _continueButton, _exportToCsvButton;

        private void Awake() {
            _quitGameButton.onClick.AddListener(Application.Quit);
            _userIDInputField.onValueChanged.AddListener(value => SceneConfiguration.UserID = value);
            _sceneTypeDropdown.onValueChanged.AddListener(value => SceneConfiguration.SceneType = (SceneType)value);
            _taskTypeDropdown.onValueChanged.AddListener(value => { SceneConfiguration.TaskType = (StudyTaskType)value; });
            SceneConfiguration.SceneType.PopulateDropdown(_sceneTypeDropdown);
            SceneConfiguration.TaskType.PopulateDropdown(_taskTypeDropdown);
            _userIDInputField.text = SceneConfiguration.UserID;
            _continueButton.onClick.AddListener(() => {
                if (int.TryParse(SceneConfiguration.UserID, out int result) && result > 0) {
                    SceneManager.LoadScene("Phase 1");
                }
            });
            _exportToCsvButton.onClick.AddListener(DataCollectionManager.ExportToCsv);
        }
    }
}
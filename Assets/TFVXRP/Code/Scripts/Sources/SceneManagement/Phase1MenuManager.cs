using Support.Extensions;
using TFVXRP.DataCollection;
using TFVXRP.DataCollection.Savegames;
using TFVXRP.Drone;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TFVXRP.SceneManagement {
    internal class Phase1MenuManager : MonoBehaviour {
        [SerializeField]
        private TMP_InputField _firstAccuracyInputField,
            _secondAccuracyInputField,
            _thirdAccuracyInputField,
            _fourthAccuracyInputField,
            _fifthAccuracyInputField,
            _sixthAccuracyInputField,
            _seventhAccuracyInputField,
            _eighthAccuracyInputField;

        [SerializeField]
        private TMP_Dropdown _firstLandingDirectionDropdown,
            _secondLandingDirectionDropdown,
            _thirdLandingDirectionDropdown,
            _fourthLandingDirectionDropdown,
            _fifthLandingDirectionDropdown,
            _sixthLandingDirectionDropdown,
            _seventhLandingDirectionDropdown,
            _eighthLandingDirectionDropdown;

        [SerializeField]
        private Button _saveAccuraciesManuallyButton;

        [SerializeField]
        private TMP_Text _logText;

        [SerializeField]
        private Button _logTheCrashButton, _connectToDroneButton, _resetDroneLandingAttemptButton, _droneBatteryButton, _backToMainMenuWithoutSaveButton, _backToMainMenuButton;

        private void Awake() {
            const CompassDirection temp = CompassDirection.Invalid;
            temp.PopulateDropdown(_firstLandingDirectionDropdown);
            temp.PopulateDropdown(_secondLandingDirectionDropdown);
            temp.PopulateDropdown(_thirdLandingDirectionDropdown);
            temp.PopulateDropdown(_fourthLandingDirectionDropdown);
            temp.PopulateDropdown(_fifthLandingDirectionDropdown);
            temp.PopulateDropdown(_sixthLandingDirectionDropdown);
            temp.PopulateDropdown(_seventhLandingDirectionDropdown);
            temp.PopulateDropdown(_eighthLandingDirectionDropdown);
            _firstAccuracyInputField.text = "INVALID";
            _secondAccuracyInputField.text = "INVALID";
            _thirdAccuracyInputField.text = "INVALID";
            _fourthAccuracyInputField.text = "INVALID";
            _fifthAccuracyInputField.text = "INVALID";
            _sixthAccuracyInputField.text = "INVALID";
            _seventhAccuracyInputField.text = "INVALID";
            _eighthAccuracyInputField.text = "INVALID";
            _saveAccuraciesManuallyButton.onClick.AddListener(() => {
                if (int.TryParse(_firstAccuracyInputField.text, out int firstLandingDistance) && int.TryParse(_secondAccuracyInputField.text, out int secondLandingDistance) &&
                    int.TryParse(_thirdAccuracyInputField.text, out int thirdLandingDistance) && int.TryParse(_fourthAccuracyInputField.text, out int fourthLandingDistance) &&
                    int.TryParse(_fifthAccuracyInputField.text, out int fifthLandingDistance) && int.TryParse(_sixthAccuracyInputField.text, out int sixthLandingDistance) &&
                    int.TryParse(_seventhAccuracyInputField.text, out int seventhLandingDistance) && int.TryParse(_eighthAccuracyInputField.text, out int eighthLandingDistance)) {
                    DataCollectionManager.SaveManualCollectedAccuracies(
                        new[] {
                            (firstLandingDistance, (CompassDirection)_firstLandingDirectionDropdown.value),
                            (secondLandingDistance, (CompassDirection)_secondLandingDirectionDropdown.value),
                            (thirdLandingDistance, (CompassDirection)_thirdLandingDirectionDropdown.value),
                            (fourthLandingDistance, (CompassDirection)_fourthLandingDirectionDropdown.value),
                            (fifthLandingDistance, (CompassDirection)_fifthLandingDirectionDropdown.value),
                            (sixthLandingDistance, (CompassDirection)_sixthLandingDirectionDropdown.value),
                            (seventhLandingDistance, (CompassDirection)_seventhLandingDirectionDropdown.value),
                            (eighthLandingDistance, (CompassDirection)_eighthLandingDirectionDropdown.value)
                        }
                    );
                    _logText.text = "Save successful";
                }
                else {
                    _logText.text = "Save unsuccessful";
                }
            });
            _logTheCrashButton.onClick.AddListener(() => DataCollectionManager.Log(DataCollectionManager.LogType.SaveCrashShot));
            _connectToDroneButton.onClick.AddListener(TelloDroneManager.EstablishDroneConnection);
            _resetDroneLandingAttemptButton.onClick.AddListener(TelloDrone.ResetLandingAttempt);
            _droneBatteryButton.onClick.AddListener(TelloDrone.BatteryLevel);
            _backToMainMenuWithoutSaveButton.onClick.AddListener(() => SceneManager.LoadScene("Phase 1 Main Menu"));
            _backToMainMenuButton.onClick.AddListener(() => {
                DataCollectionManager.Log(DataCollectionManager.LogType.CalculateResults);
                SceneManager.LoadScene("Phase 1 Main Menu");
            });
        }
    }
}
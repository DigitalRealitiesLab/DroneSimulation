using System;
using System.Collections.Generic;
using Support.UserInput.InputTypes;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Support.UserInput {
    [RequireComponent(typeof(PlayerInput))]
    internal sealed partial class InputManager : MonoBehaviour {
        private const float _STICK_DRIFT_THRESHOLD = 0.25f;

        [SerializeField]
        private bool _useStickThreshold = true;

#if UNITY_EDITOR
        private void Reset() {
            var playerInput = GetComponent<PlayerInput>();
            string inputActionsAssetGuid = AssetDatabase.FindAssets("t:InputActionAsset", new[] { "Packages/com.p.jh" })[0];
            string inputActionsAssetPath = AssetDatabase.GUIDToAssetPath(inputActionsAssetGuid);
            playerInput.actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(inputActionsAssetPath);
        }
#endif

        private void OnStick(InputValue value, IDictionary<XBoxControllerInput, List<Action<Vector2>>> listener, XBoxControllerInput key) {
            if (!listener.ContainsKey(key)) {
                return;
            }

            Vector2 valueAsVector2 = value.AsVector2();
            var input = new Vector2(
                _useStickThreshold && Mathf.Abs(valueAsVector2.x) <= _STICK_DRIFT_THRESHOLD ? 0 : valueAsVector2.x,
                _useStickThreshold && Mathf.Abs(valueAsVector2.y) <= _STICK_DRIFT_THRESHOLD ? 0 : valueAsVector2.y
            );
            foreach (Action<Vector2> action in listener[key]) {
                action(input);
            }
        }

        private void OnLeftStick(InputValue value) => OnStick(value, _stickListener, XBoxControllerInput.LStick);

        private void OnRightStick(InputValue value) => OnStick(value, _stickListener, XBoxControllerInput.RStick);

        private static void OnButton(IDictionary<XBoxControllerInput, List<Action>> listener, XBoxControllerInput key) {
            if (!listener.ContainsKey(key)) {
                return;
            }

            foreach (Action action in listener[key]) {
                action();
            }
        }

        private void OnAButton() => OnButton(_buttonListener, XBoxControllerInput.A);

        private void OnBButton() => OnButton(_buttonListener, XBoxControllerInput.B);

        private void OnXButton() => OnButton(_buttonListener, XBoxControllerInput.X);

        private void OnYButton() => OnButton(_buttonListener, XBoxControllerInput.Y);

        private void OnMenuButton() => OnButton(_buttonListener, XBoxControllerInput.Menu);
    }
}
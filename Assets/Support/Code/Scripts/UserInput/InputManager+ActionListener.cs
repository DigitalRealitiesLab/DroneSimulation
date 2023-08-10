using System;
using System.Collections.Generic;
using Support.Extensions;
using Support.UserInput.InputTypes;
using UnityEngine;

namespace Support.UserInput {
    internal partial class InputManager : IInputManager {
        private const XBoxControllerInput _STICKS = XBoxControllerInput.LStick | XBoxControllerInput.RStick;

        private const XBoxControllerInput _BUTTONS = XBoxControllerInput.A | XBoxControllerInput.B | XBoxControllerInput.X | XBoxControllerInput.Y | XBoxControllerInput.LB | XBoxControllerInput.RB |
                                                     XBoxControllerInput.Menu;

        private readonly Dictionary<XBoxControllerInput, List<Action>> _buttonListener = new();
        private readonly Dictionary<XBoxControllerInput, List<Action<Vector2>>> _stickListener = new();

        bool IInputManager.AddListener(XBoxControllerInput input, Action<Vector2> callback) => AddListener(_STICKS, callback, _stickListener, input);

        bool IInputManager.AddListener(XBoxControllerInput input, Action callback) => AddListener(_BUTTONS, callback, _buttonListener, input);

        private static bool AddListener<TAction>(XBoxControllerInput flagsToCheckAgainst, TAction callback, IDictionary<XBoxControllerInput, List<TAction>> listener, XBoxControllerInput input) {
            if (!input.HasFlags(flagsToCheckAgainst)) {
                return false;
            }

            AddCallbackToListenerForInput();
            return true;

            void AddCallbackToListenerForInput() {
                foreach (XBoxControllerInput xBoxControllerInput in input.ForEach()) {
                    if (listener.TryGetValue(xBoxControllerInput, out List<TAction> value)) {
                        value.Add(callback);
                    }
                    else {
                        listener[xBoxControllerInput] = new List<TAction> { callback };
                    }
                }
            }
        }
    }
}
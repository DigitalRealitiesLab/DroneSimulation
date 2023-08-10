using System;
using Support.UserInput.InputTypes;
using UnityEngine;

namespace Support.UserInput {
    public interface IInputManager {
        public bool AddListener(XBoxControllerInput input, Action<Vector2> callback);

        public bool AddListener(XBoxControllerInput input, Action callback);
    }
}
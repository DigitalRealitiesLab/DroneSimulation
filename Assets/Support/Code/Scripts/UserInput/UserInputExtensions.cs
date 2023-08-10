using System.Linq;
using Support.Extensions;
using Support.UserInput.InputTypes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Support.UserInput {
    internal static class UserInputExtensions {
        internal static Vector2 AsVector2(this InputValue value) => value.Get<Vector2>();

        internal static bool HasFlags(this XBoxControllerInput input, XBoxControllerInput valueToCompareAgainst) =>
            input.ForEach().Any(valueToCompareAgainst.HasFlag);
    }
}
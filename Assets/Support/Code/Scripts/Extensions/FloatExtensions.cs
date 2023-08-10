using UnityEngine;

namespace Support.Extensions {
    public static class FloatExtensions {
        private const float _COMPARISON_ACCURACY = .0001f;

        public static bool IsEqualTo(this float self, float other) => Mathf.Abs(self - other) < _COMPARISON_ACCURACY;

        public static bool IsNotEqualTo(this float self, float other) => !self.IsEqualTo(other);
    }
}
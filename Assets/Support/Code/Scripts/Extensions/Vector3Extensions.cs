using UnityEngine;

namespace Support.Extensions {
    public static class Vector3Extensions {
        public static Vector3 Rotate(this Vector3 vector, Quaternion rotation) => rotation * vector;
    }
}
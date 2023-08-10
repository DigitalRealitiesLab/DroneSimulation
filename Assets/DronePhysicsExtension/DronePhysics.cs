using UnityEngine;
using YueUltimateDronePhysics;

namespace DronePhysicsExtension {
    [RequireComponent(typeof(InputModule))]
    internal class DronePhysics : YueDronePhysics {
        private new void Start() {
            base.Start();

            inputModule.Startup += () => { armed = true; };
            inputModule.Landed += () => { armed = false; };
        }

        internal void ApplyTurbulence(Vector3 turbulenceForce, Vector3 position) => rb.AddForceAtPosition(turbulenceForce, position);
    }
}
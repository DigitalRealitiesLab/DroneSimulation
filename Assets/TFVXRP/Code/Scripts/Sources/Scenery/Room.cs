using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace TFVXRP.Scenery {
    internal class Room : MonoBehaviour, IRoom {
        [SerializeField]
        private Material _transparentWithShadowsMaterial, _occlusionMaterial;

        [SerializeField]
        private GameObject[] _topFacingPlanesWithShadows;

        private IRoom This => this;

        private void Start() {
            (bool SceneryIsReal, bool VehicleIsReal) sceneryRealism = This.GetSceneryRealism.Invoke();
            if (!sceneryRealism.SceneryIsReal) {
                return;
            }

            if (sceneryRealism.VehicleIsReal) {
                foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>(true)) {
                    meshRenderer.enabled = false;
                }

                foreach (MeshCollider meshCollider in GetComponentsInChildren<MeshCollider>(true)) {
                    meshCollider.enabled = false;
                }
            }
            else {
                foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>(true)) {
                    bool isTopFacingPlane = _topFacingPlanesWithShadows.Contains(meshRenderer.gameObject);
                    meshRenderer.material = isTopFacingPlane ? _transparentWithShadowsMaterial : _occlusionMaterial;
                    meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                }
            }
        }

        Func<(bool, bool)> IRoom.GetSceneryRealism { get; set; }
    }
}
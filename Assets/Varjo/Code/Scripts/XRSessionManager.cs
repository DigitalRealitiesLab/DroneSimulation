using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.XR.Management;
using Varjo.XR;

namespace Varjo {
    internal class XRSessionManager : MonoBehaviour {
        private static readonly int _CAM_WB_GAINS = Shader.PropertyToID("_CamWBGains");
        private static readonly int _CAM_INV_CCM = Shader.PropertyToID("_CamInvCCM");
        private static readonly int _CAM_CCM = Shader.PropertyToID("_CamCCM");

        [SerializeField]
        private Camera _xrCamera;

        [Header("Mixed Reality Features")]
        [SerializeField]
        private bool _videoSeeThrough = true;

        [SerializeField]
        private bool _depthEstimation;

        [SerializeField]
        [Range(0f, 1.0f)]
        private float _vrEyeOffset = 1.0f;

        [Header("Real Time Environment")]
        [SerializeField]
        private bool _environmentReflections;

        [SerializeField]
        private int _reflectionRefreshRate = 30;

        [SerializeField]
        private VolumeProfile _skyboxProfile;

        [SerializeField]
        private Cubemap _defaultSky;

        [SerializeField]
        private CubemapEvent _onCubemapUpdate = new();

        private VarjoCameraSubsystem _cameraSubsystem;
        private bool _cubemapEventListenerSet;

        private VarjoEnvironmentCubemapStream.VarjoEnvironmentCubemapFrame _cubemapFrame;
        private float _currentVREyeOffset = 1f;

        private bool _defaultSkyActive;
        private bool _depthEstimationEnabled;
        private bool _environmentReflectionsEnabled;

        private HDAdditionalCameraData _hdCameraData;
        private VarjoCameraMetadataStream.VarjoCameraMetadataFrame _metadataFrame;

        private bool _metadataStreamEnabled;
        private bool _originalDepthSortingValue;

        private bool _originalOpaqueValue;
        private bool _originalSubmitDepthValue;

        private bool _videoSeeThroughEnabled;
        private Exposure _volumeExposure;

        private HDRISky _volumeSky;
        private VSTWhiteBalance _volumeVstWhiteBalance;

        private void Start() {
            if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null) {
                var loader = XRGeneralSettings.Instance.Manager.activeLoader as VarjoLoader;
                if (loader != null) {
                    _cameraSubsystem = loader.cameraSubsystem as VarjoCameraSubsystem;
                }
            }

            _cameraSubsystem?.Start();

            _originalOpaqueValue = VarjoRendering.GetOpaque();
            VarjoRendering.SetOpaque(false);
            _cubemapEventListenerSet = _onCubemapUpdate.GetPersistentEventCount() > 0;
            _hdCameraData = _xrCamera.GetComponent<HDAdditionalCameraData>();

            if (!_skyboxProfile.TryGet(out _volumeSky)) {
                _volumeSky = _skyboxProfile.Add<HDRISky>(true);
            }

            if (!_skyboxProfile.TryGet(out _volumeExposure)) {
                _volumeExposure = _skyboxProfile.Add<Exposure>(true);
            }

            if (!_skyboxProfile.TryGet(out _volumeVstWhiteBalance)) {
                _volumeVstWhiteBalance = _skyboxProfile.Add<VSTWhiteBalance>(true);
            }
        }

        private void Update() => UpdateMrFeatures();

        private void OnDisable() {
            _videoSeeThrough = false;
            _depthEstimation = false;
            _environmentReflections = false;
            UpdateMrFeatures();
            VarjoRendering.SetOpaque(_originalOpaqueValue);
        }

        private void UpdateMrFeatures() {
            UpdateVideoSeeThrough();
            UpdateDepthEstimation();
            UpdateVREyeOffSet();
            UpdateEnvironmentReflections();

            void UpdateVideoSeeThrough() {
                if (_videoSeeThrough == _videoSeeThroughEnabled) {
                    return;
                }

                if (_videoSeeThrough) {
                    _videoSeeThrough = VarjoMixedReality.StartRender();
                    if (_hdCameraData) {
                        _hdCameraData.clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
                    }
                }
                else {
                    VarjoMixedReality.StopRender();
                    if (_hdCameraData) {
                        _hdCameraData.clearColorMode = HDAdditionalCameraData.ClearColorMode.Sky;
                    }
                }

                _videoSeeThroughEnabled = _videoSeeThrough;
            }

            void UpdateDepthEstimation() {
                if (_depthEstimation == _depthEstimationEnabled) {
                    return;
                }

                if (_depthEstimation) {
                    _depthEstimation = VarjoMixedReality.EnableDepthEstimation();
                    _originalSubmitDepthValue = VarjoRendering.GetSubmitDepth();
                    _originalDepthSortingValue = VarjoRendering.GetDepthSorting();
                    VarjoRendering.SetSubmitDepth(true);
                    VarjoRendering.SetDepthSorting(true);
                }
                else {
                    VarjoMixedReality.DisableDepthEstimation();
                    VarjoRendering.SetSubmitDepth(_originalSubmitDepthValue);
                    VarjoRendering.SetDepthSorting(_originalDepthSortingValue);
                }

                _depthEstimationEnabled = _depthEstimation;
            }

            void UpdateVREyeOffSet() {
                if (Math.Abs(_vrEyeOffset - _currentVREyeOffset) < .0000f) {
                    return;
                }

                VarjoMixedReality.SetVRViewOffset(_vrEyeOffset);
                _currentVREyeOffset = _vrEyeOffset;
            }

            void UpdateEnvironmentReflections() {
                if (_environmentReflections != _environmentReflectionsEnabled) {
                    if (_environmentReflections) {
                        if (VarjoMixedReality.environmentCubemapStream.IsSupported()) {
                            _environmentReflections = VarjoMixedReality.environmentCubemapStream.Start();
                        }

                        if (!_cameraSubsystem.IsMetadataStreamEnabled) {
                            _cameraSubsystem.EnableMetadataStream();
                        }

                        _metadataStreamEnabled = _cameraSubsystem.IsMetadataStreamEnabled;
                    }
                    else {
                        VarjoMixedReality.environmentCubemapStream.Stop();
                        _cameraSubsystem.DisableMetadataStream();
                    }

                    _environmentReflectionsEnabled = _environmentReflections;
                }

                if (_environmentReflectionsEnabled && _metadataStreamEnabled) {
                    if (!VarjoMixedReality.environmentCubemapStream.hasNewFrame || !_cameraSubsystem.MetadataStream.hasNewFrame) {
                        return;
                    }

                    _cubemapFrame = VarjoMixedReality.environmentCubemapStream.GetFrame();

                    _metadataFrame = _cameraSubsystem.MetadataStream.GetFrame();
                    float exposureValue = (float)_metadataFrame.metadata.ev + Mathf.Log((float)_metadataFrame.metadata.cameraCalibrationConstant, 2f);
                    _volumeExposure.fixedExposure.Override(exposureValue);

                    _volumeSky.hdriSky.Override(_cubemapFrame.cubemap);
                    _volumeSky.updateMode.Override(EnvironmentUpdateMode.Realtime);
                    _volumeSky.updatePeriod.Override(1f / _reflectionRefreshRate);
                    _defaultSkyActive = false;

                    _volumeVstWhiteBalance.intensity.Override(1f);

                    // Set white balance normalization values
                    Shader.SetGlobalColor(_CAM_WB_GAINS, _metadataFrame.metadata.wbNormalizationData.wbGains);
                    Shader.SetGlobalMatrix(_CAM_INV_CCM, _metadataFrame.metadata.wbNormalizationData.invCCM);
                    Shader.SetGlobalMatrix(_CAM_CCM, _metadataFrame.metadata.wbNormalizationData.ccm);

                    if (_cubemapEventListenerSet) {
                        _onCubemapUpdate.Invoke();
                    }
                }
                else if (!_defaultSkyActive) {
                    _volumeSky.hdriSky.Override(_defaultSky);
                    _volumeSky.updateMode.Override(EnvironmentUpdateMode.OnChanged);
                    _volumeExposure.fixedExposure.Override(6.5f);
                    _volumeVstWhiteBalance.intensity.Override(0f);
                    _defaultSkyActive = true;
                }
            }
        }

        [Serializable]
        private class CubemapEvent : UnityEvent { }
    }
}
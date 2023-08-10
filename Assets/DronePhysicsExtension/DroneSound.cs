using UnityEngine;

namespace DronePhysicsExtension {
    public class DroneSound : MonoBehaviour {
        [SerializeField]
        [Range(0f, 1f)]
        private float _overallVolume = .5f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _pitchModifier = .1f;

        [SerializeField]
        private AudioSource _startupAudioSource, _flyingAudioSource, _shutdownAudioSource;

        [SerializeField]
        private InputModule _droneInput;

        private double _flyingAudioClipLength;
        private double _flyingAudioClipStart;

        private void Start() {
            _droneInput.Startup += () => {
                _flyingAudioClipStart = AudioSettings.dspTime;
                _startupAudioSource.Play();
                AudioClip clip = _startupAudioSource.clip;
                _flyingAudioClipLength = (double)clip.samples / clip.frequency;
                _flyingAudioSource.volume = 0;
                _flyingAudioSource.Play();
            };
            _droneInput.TouchdownImminent += () => {
                if (_shutdownAudioSource.isPlaying) {
                    return;
                }

                _shutdownAudioSource.Play();
                _startupAudioSource.Stop();
                _flyingAudioSource.Stop();
            };
            _droneInput.Collided += () => {
                _startupAudioSource.Stop();
                _flyingAudioSource.Stop();
                _shutdownAudioSource.Stop();
            };
        }

        private void Update() {
            SetVolume();
            _flyingAudioSource.pitch = 1 + _droneInput.rawThrust * _pitchModifier;
        }

        private void SetVolume() {
            double volume = _startupAudioSource.isPlaying ? (AudioSettings.dspTime - _flyingAudioClipStart) / _flyingAudioClipLength : 0;
            _startupAudioSource.volume = Mathf.Lerp(1, 0, (float)volume);
            _flyingAudioSource.volume = _startupAudioSource.isPlaying ? Mathf.Lerp(0, _overallVolume, (float)volume) : _shutdownAudioSource.isPlaying ? 0 : _overallVolume;
            _shutdownAudioSource.volume = _overallVolume * 1.5f;
        }
    }
}
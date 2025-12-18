/*using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Serialization;

namespace Foxbyte.Core.Components.CinemachineTools
{
    [RequireComponent(typeof(CinemachineSplineDolly))]
    public class EasingDollyController : MonoBehaviour
    {
        [Header("Dolly Settings")]
        [Tooltip("Total duration (in seconds) to move along the spline.")]
        public float AnimationDuration = 5f;

        [Tooltip("Fraction of the journey used for easing in (0 to 1).")]
        [Range(0f, 1f)]
        public float EaseIn = 0.1f;

        [Tooltip("Fraction of the journey used for easing out (0 to 1).")]
        [Range(0f, 1f)]
        public float EaseOut = 0.1f;

        [FormerlySerializedAs("visibilityEvents")]
        [Header("Mesh Visibility Events")]
        [SerializeField]
        private MeshVisibilityEvent[] _visibilityEvents;

        private CinemachineSplineDolly _dolly;
        private float _elapsedTime = 0f;
        private bool _isPlaying = false;

        // Define a serializable class for visibility events
        [System.Serializable]
        public class MeshVisibilityEvent
        {
            public MeshRenderer meshRenderer;

            [Tooltip("Time in seconds when the mesh should become visible")]
            public float startTime;

            [Tooltip("Time in seconds when the mesh should become invisible")]
            public float endTime;
        }

        private void Awake()
        {
            _dolly = GetComponent<CinemachineSplineDolly>();
            if (_dolly == null)
            {
                Debug.LogError("EasingDollyController: No CinemachineSplineDolly component found on this GameObject.");
            }

            // Ensure all meshes start disabled if before their start time
            UpdateMeshVisibility(0f);
        }

        private void Update()
        {
            // Start the animation with the spacebar.
            if (!_isPlaying && Input.GetKeyDown(KeyCode.Space))
            {
                Play();
            }

            if (!_isPlaying || _dolly == null)
                return;

            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / AnimationDuration);

            // Compute the eased progress using the cubic easing functions.
            float easedT = GetEasedProgress(t);

            // Update the spline dolly position (in normalized mode).
            _dolly.CameraPosition = easedT;

            // Update mesh visibility based on current time
            UpdateMeshVisibility(_elapsedTime);

            if (t >= 1f)
            {
                _isPlaying = false;
            }
        }

        public void Play()
        {
            _elapsedTime = 0f;
            _isPlaying = true;
            UpdateMeshVisibility(0f); // Reset visibility at start
        }

        private void UpdateMeshVisibility(float currentTime)
        {
            if (_visibilityEvents == null) return;

            foreach (var visibilityEvent in _visibilityEvents)
            {
                if (visibilityEvent.meshRenderer != null)
                {
                    bool shouldBeVisible = currentTime >= visibilityEvent.startTime &&
                                           currentTime <= visibilityEvent.endTime;
                    visibilityEvent.meshRenderer.enabled = shouldBeVisible;
                }
            }
        }

        private float GetEasedProgress(float t)
        {
            if (t < EaseIn)
            {
                float u = t / EaseIn;
                return EaseIn * (2f * u * u - u * u * u);
            }
            else if (t > 1f - EaseOut)
            {
                float v = (t - (1f - EaseOut)) / EaseOut;
                return (1f - EaseOut) + EaseOut * (-v * v * v + v * v + v);
            }
            else
            {
                return t;
            }
        }
    }
}*/
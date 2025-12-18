using System.Collections;
using FHH.Logic.Cesium;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.Logic.VR
{
    /// <summary>
    /// Fades a UI Document in and out based on the movement speed of a target Transform.
    /// </summary>
    public class FadeOnMove : MonoBehaviour
    {
        [SerializeField] UIDocument _uiDocument;
        [SerializeField] Transform _target;

        [Header("Speed → Opacity Mapping")]
        [Tooltip("Speed (m/s) at or below which the UI is fully opaque.")]
        public float MinSpeed = 0.02f;

        [Tooltip("Speed (m/s) at or above which the UI is fully transparent.")]
        public float MaxSpeed = 0.4f;

        [Tooltip("Lower bound of opacity when moving fast (usually 0).")]
        public float MinOpacity = 0f;

        [Tooltip("Upper bound of opacity when still (usually 1).")]
        public float MaxOpacity = 1f;

        [Header("Smoothing")]
        [Tooltip("Time (seconds) for opacity to smooth toward the target value.")]
        public float OpacitySmoothTime = 0.08f;

        [Tooltip("Optional extra ease on the speed→opacity curve. 0 = linear, 1 = smoothstep, 2 = smootherstep.")]
        [Range(0, 2)] public int Ease = 1;

        [Header("Delay")] 
        [Range(0f,10f)]
        public float DelayAtStartup = 5f;
        [Tooltip("Seconds to wait before increasing opacity (fading in). 0 = no delay.")]
        public float FadeInDelay = 0.3f;

        [SerializeField] private CesiumTilesLoadingProgressProvider _cesiumTilesLoadingProgress;
        private bool _isFadingAllowed;

        private VisualElement _root;
        private Vector3 _lastPos;
        private bool _hasLast;
        private float _currentOpacity = 1f;
        private float _opacityVel;

        private float _fadeInTimer;

        void Awake()
        {
            _root = _uiDocument != null ? _uiDocument.rootVisualElement : null;
            if (_root != null)
            {
                _root.style.opacity = 0f;
            }
        }

        void OnEnable()
        {
            if (_root != null)
            {
                // Initialize from whatever style is currently set (fallback to 1 if unset)
                //_currentOpacity = Mathf.Clamp01(_root.resolvedStyle.opacity <= 0f ? 1f : _root.resolvedStyle.opacity);
                //_root.style.opacity = _currentOpacity;
            }
            _hasLast = false; // force capture on first LateUpdate
            _fadeInTimer = 0f;
            _cesiumTilesLoadingProgress.TilesLoaded += OnTilesLoaded;
        }

        private void OnDisable()
        {
            _cesiumTilesLoadingProgress.TilesLoaded -= OnTilesLoaded;
        }

        void OnTilesLoaded()
        {
            //_isFadingAllowed = true;
            StartCoroutine(DelayedStart());
        }

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(DelayAtStartup);
            _isFadingAllowed = true;
        }

        void LateUpdate()
        {
            if (!_isFadingAllowed)
                return;
            if (_root == null || _target == null)
                return;

            float dt = Time.unscaledDeltaTime;
            if (dt <= 0f)
                return;

            Vector3 pos = _target.position;

            if (!_hasLast)
            {
                _lastPos = pos;
                _hasLast = true;
                return;
            }

            // Compute linear speed (m/s)
            float speed = Vector3.Distance(pos, _lastPos) / dt;
            _lastPos = pos;

            // Normalize speed to [0,1]
            float t = Mathf.InverseLerp(MinSpeed, MaxSpeed, speed); // 0 = still, 1 = fast

            switch (Ease)
            {
                case 1: t = t * t * (3f - 2f * t); break; // smoothstep
                case 2: t = t * t * t * (t * (6f * t - 15f) + 10f); break; // smootherstep
                // case 0: linear
            }

            // Map to opacity (invert so fast = low opacity)
            float desiredOpacity = Mathf.Lerp(MaxOpacity, MinOpacity, t);

            // Delay only when we want to increase opacity (fade in)
            bool wantsHigherOpacity = desiredOpacity > _currentOpacity + 0.0001f;

            if (wantsHigherOpacity && FadeInDelay > 0f)
            {
                _fadeInTimer += dt;
                if (_fadeInTimer < FadeInDelay)
                {
                    // Hold current opacity until the delay elapses
                    desiredOpacity = _currentOpacity;
                }
            }
            else
            {
                // Reset the timer whenever we are not trying to fade in
                _fadeInTimer = 0f;
            }

            // Smooth opacity to avoid flicker
            _currentOpacity = Mathf.SmoothDamp(_currentOpacity, desiredOpacity, ref _opacityVel, OpacitySmoothTime, Mathf.Infinity, dt);

            _root.style.opacity = _currentOpacity;
        }

        public void ForceShow()
        {
            _currentOpacity = MaxOpacity;
            _opacityVel = 0f;
            _fadeInTimer = 0f;
            if (_root != null) _root.style.opacity = _currentOpacity;
        }

        public void ForceHide()
        {
            _currentOpacity = MinOpacity;
            _opacityVel = 0f;
            _fadeInTimer = 0f;
            if (_root != null) _root.style.opacity = _currentOpacity;
        }
    }
}
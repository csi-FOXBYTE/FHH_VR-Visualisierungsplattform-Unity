using CesiumForUnity;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace FHH.Logic.Cesium
{
    /// <summary>
    /// Exposes loading progress of all active Cesium3DTileset instances in the scene.
    /// Declares 'loaded' when all active tilesets are >= CompleteThreshold, or when
    /// progress is stable for ReadyDelay seconds near completion.
    /// </summary>
    [DisallowMultipleComponent]
    public class CesiumTilesLoadingProgressProvider : MonoBehaviour
    {
        public event Action<float> TilesLoadingProgressChanged;
        public event Action TilesLoaded;
        public UnityEvent TilesLoadedEvent;

        [Header("Timing")]
        [Min(0f)] public float ReadyDelay = 1.1f; // stable-time needed before we fire 'loaded'
        [SerializeField, Range(70f, 100f)]
        private float _completeThreshold = 99.5f; // consider complete when all active >= this

        [Header("Debug")]
        [SerializeField] private bool _debug = false;

        public float Progress { get; private set; }

        private Cesium3DTileset[] _tileSets = Array.Empty<Cesium3DTileset>();
        private float _lastSent = -1f; // last progress we emitted (percentage)
        private bool _isLoading;
        private float _stableTimer; // time spent without significant change
        private float _lastForStability = -1f;  // last value used for stability detection

        private bool _hasCompleted;

        private void Awake()
        {
            // do nothing until explicitly started
            enabled = false;
            // Avoid null UnityEvent crashes
            if (TilesLoadedEvent == null) TilesLoadedEvent = new UnityEvent();
        }

        /// <summary>
        /// Called from LayerManager when tiles are (re)built, then disables itself when done.
        /// </summary>
        public void StartLoadingProgressCalculation()
        {
            if (_hasCompleted) return;
            _tileSets = FindObjectsByType<Cesium3DTileset>(FindObjectsSortMode.None) ?? Array.Empty<Cesium3DTileset>();
            _isLoading = _tileSets.Length > 0;
            _lastSent = -1f;
            _stableTimer = 0f;
            _lastForStability = -1f;
            enabled = _isLoading;
            if (_debug) Debug.Log($"ProgressProvider: tracking {_tileSets.Length} tileset(s).");
        }

        private void Update()
        {
            if (!_isLoading) return;

            var active = 0;
            var sum = 0f;
            var allAtOrAbove = true;

            foreach (var ts in _tileSets)
            {
                if (!ts || !ts.enabled || !ts.gameObject.activeInHierarchy) continue;

                float p = 0f;
                try
                {
                    p = ts.ComputeLoadProgress(); // 0..100 percent
                }
                catch
                {
                    /* some tilesets may briefly be invalid during recreate */
                }

                if (float.IsNaN(p) || float.IsInfinity(p)) continue;

                p = Mathf.Clamp(p, 0f, 100f);
                sum += p;
                active++;

                if (p < _completeThreshold) allAtOrAbove = false;
            }

            // if no active tilesets, treat as loaded
            Progress = active > 0 ? sum / active : 100f;

            if (_debug) Debug.Log($"Loading progress: {Mathf.Floor(Progress)}% (active={active})");

            // Emit progress changes (avoid spam)
            var floored = Mathf.Floor(Progress);
            if (Mathf.Abs(floored - _lastSent) >= 1f)
            {
                _lastSent = floored;
                try
                {
                    TilesLoadingProgressChanged?.Invoke(floored);
                }
                catch 
                    (Exception ex) { if (_debug) Debug.LogError(ex); }
            }

            // Stability detection: if progress barely moves, consider it "ready" after ReadyDelay.
            if (Mathf.Abs(Progress - _lastForStability) < 0.1f)
                _stableTimer += Time.deltaTime;
            else
            {
                _stableTimer = 0f;
                _lastForStability = Progress;
            }

            // Complete if all active tilesets at/above threshold, OR
            // progress stable at high value for ReadyDelay seconds
            var stableEnough = (_stableTimer >= ReadyDelay) && (Progress >= (_completeThreshold - 3f));

            if (allAtOrAbove || stableEnough)
            {
                FinishOnceAsync().Forget();
                //DelayedLoadedEventAsync().Forget();
                //if (_debug) Debug.Log("All Cesium TileSets considered loaded.");
                //_isLoading = false;
                //enabled = false;
            }
        }

        private async UniTaskVoid FinishOnceAsync()
        {
            _hasCompleted = true; 
            _isLoading = false;
            enabled = false;

            // Free references so we don’t observe later Cesium changes
            _tileSets = Array.Empty<Cesium3DTileset>();

            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            try { TilesLoaded?.Invoke(); } catch (Exception ex) { if (_debug) Debug.LogError(ex); }

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            try { TilesLoadedEvent?.Invoke(); } catch (Exception ex) { if (_debug) Debug.LogError(ex); }
        }

    }
}
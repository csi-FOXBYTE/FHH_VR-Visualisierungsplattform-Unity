using System;
using System.Collections.Generic;
using UnityEngine;
using CesiumForUnity;

namespace FHH.Logic.Components.CameraTools
{
    [DisallowMultipleComponent]
    public class LayerDistanceCulling : MonoBehaviour
    {
        [Serializable]
        public struct LayerRange
        {
            [Tooltip("Unity layer name")]
            public string LayerName;

            [Min(0f)] public float MinDistance; // at MinHeight
            [Min(0f)] public float MaxDistance; // at MaxHeight
        }

        [Header("Camera")]
        public Camera TargetCamera;

        [Header("XR Rig / Cesium")]
        [Tooltip("XR rig that has a CesiumGlobeAnchor; we read .height (meters)")]
        public CesiumGlobeAnchor XrRigGlobeAnchor;

        [Tooltip("Optional offset in meters added to the anchor height")]
        public double HeightOffsetMeters = 0.0;

        [Header("Height → Distance mapping")]
        [Tooltip("Height (m) where MinDistance applies")]
        [Min(0f)]
        public double MinHeight = 2;

        [Tooltip("Height (m) where MaxDistance applies")]
        [Min(0.01f)]
        public double MaxHeight = 50.0; // flying

        [Tooltip("Remap curve: x = normalized height 0..1, y = remapped 0..1")]
        public AnimationCurve HeightResponse = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Per-layer distance ranges")] public List<LayerRange> Layers = new List<LayerRange>();

        [Header("Behavior")]
        [Tooltip("Keep existing distances for layers not listed (0 = use camera far plane)")]
        public bool PreserveUnspecifiedLayers = true;

        [Tooltip("Smoothing strength (0 = instant). Higher = smoother/slower")]
        [Range(0f, 10f)]
        public float Smoothing = 3f;

        [Tooltip("Update distances in LateUpdate (after all movement)")]
        public bool UpdateInLateUpdate = true;

        private Camera _cam;
        private readonly float[] _currentDistances = new float[32];
        private bool _initialized;
        private int[] _layerIndices;

        void Awake()
        {
            _cam = TargetCamera != null ? TargetCamera : GetComponent<Camera>();
            if (_cam == null) _cam = Camera.main;

            if (_cam == null)
            {
                Debug.LogError($"{nameof(LayerDistanceCulling)}: No Camera found. Assign one or place this on a Camera.");
                enabled = false;
                return;
            }

            if (XrRigGlobeAnchor == null)
            {
                Debug.LogError($"{nameof(LayerDistanceCulling)}: XrRigGlobeAnchor not set. Assign a CesiumGlobeAnchor on your XR rig.");
                enabled = false;
                return;
            }

            CacheLayerIndices();
            InitializeFromCamera();
            ApplyImmediate(); // apply at current height
            _initialized = true;
        }

        void OnEnable()
        {
            if (!_initialized) return;
            InitializeFromCamera();
            ApplyImmediate();
        }

        void OnValidate()
        {
            // Swap any accidental Min/Max inversions and refresh indices
            for (int i = 0; i < Layers.Count; i++)
            {
                if (Layers[i].MaxDistance < Layers[i].MinDistance)
                {
                    var lr = Layers[i];
                    (lr.MinDistance, lr.MaxDistance) = (lr.MaxDistance, lr.MinDistance);
                    Layers[i] = lr;
                }
            }

            CacheLayerIndices();

            if (Application.isPlaying && _cam != null && XrRigGlobeAnchor != null)
            {
                ApplyImmediate();
            }
        }

        void Update()
        {
            if (!UpdateInLateUpdate) Tick(Time.deltaTime);
        }

        void LateUpdate()
        {
            if (UpdateInLateUpdate) Tick(Time.deltaTime);
        }

        private void Tick(float dt)
        {
            if (_cam == null || XrRigGlobeAnchor == null) return;

            float t = ComputeNormalizedHeight01();

            // only touch the layers we manage; leave others as-is
            for (int i = 0; i < Layers.Count; i++)
            {
                int layerIndex = _layerIndices[i];
                if (layerIndex < 0) continue; // invalid layer name
                float desired = Mathf.Lerp(Layers[i].MinDistance, Layers[i].MaxDistance, t);

                float current = _currentDistances[layerIndex];
                float a = Smoothing <= 0f ? 1f : 1f - Mathf.Exp(-Smoothing * dt);
                _currentDistances[layerIndex] = Mathf.Lerp(current, desired, a);
            }

            // If not preserving unspecified, ensure others are zero (far plane)
            if (!PreserveUnspecifiedLayers)
            {
                // Zero the rest without allocating; single pass
                for (int li = 0; li < 32; li++)
                {
                    bool managed = false;
                    for (int i = 0; i < _layerIndices.Length; i++)
                    {
                        if (_layerIndices[i] == li)
                        {
                            managed = true;
                            break;
                        }
                    }

                    if (!managed) _currentDistances[li] = 0f;
                }
            }

            _cam.layerCullDistances = _currentDistances;
        }

        private void ApplyImmediate()
        {
            float t = ComputeNormalizedHeight01();

            if (!PreserveUnspecifiedLayers)
            {
                Array.Clear(_currentDistances, 0, _currentDistances.Length);
            }
            else
            {
                // start from whatever the camera currently has, once
                var src = _cam.layerCullDistances;
                if (src != null && src.Length == 32) Array.Copy(src, _currentDistances, 32);
            }

            for (int i = 0; i < Layers.Count; i++)
            {
                int layerIndex = _layerIndices[i];
                if (layerIndex < 0) continue;
                _currentDistances[layerIndex] = Mathf.Lerp(Layers[i].MinDistance, Layers[i].MaxDistance, t);
            }

            _cam.layerCullDistances = _currentDistances;
        }

        private float ComputeNormalizedHeight01()
        {
            // CesiumGlobeAnchor.height is meters above WGS84 ellipsoid
            double h = (XrRigGlobeAnchor.longitudeLatitudeHeight.z + HeightOffsetMeters); //(XrRigGlobeAnchor.height + HeightOffsetMeters);
            double denom = Math.Max(1e-3, (MaxHeight - MinHeight));
            float tf = Mathf.Clamp01((float)((h - MinHeight) / denom));
            return HeightResponse != null ? Mathf.Clamp01(HeightResponse.Evaluate(tf)) : tf;
        }

        private void InitializeFromCamera()
        {
            var src = _cam.layerCullDistances;
            if (src != null && src.Length == 32) Array.Copy(src, _currentDistances, 32);
            else Array.Clear(_currentDistances, 0, 32);
        }

        private void CacheLayerIndices()
        {
            if (Layers == null) Layers = new List<LayerRange>();
            int count = Layers.Count;
            if (_layerIndices == null || _layerIndices.Length != count)
            {
                _layerIndices = new int[count];
            }

            for (int i = 0; i < count; i++)
            {
                string name = Layers[i].LayerName;
                _layerIndices[i] = string.IsNullOrWhiteSpace(name) ? -1 : LayerMask.NameToLayer(name);
                if (_layerIndices[i] < 0 || _layerIndices[i] > 31)
                {
                    // keep -1; we skip invalid
#if UNITY_EDITOR
                    if (!string.IsNullOrWhiteSpace(name))
                        Debug.LogWarning($"{nameof(LayerDistanceCulling)}: Layer '{name}' not found. Skipping.");
#endif
                    _layerIndices[i] = -1;
                }
            }
        }
    }
}
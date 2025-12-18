using UnityEngine;
using CesiumForUnity;
using Unity.Mathematics;

namespace FHH.Logic.Components.Laserpointer
{
    public class LaserPointerIndicatorVisual : MonoBehaviour
    {
        [Header("References")]
        public Camera Cam;

        [SerializeField]
        private Renderer _renderer;

        [Header("Screen Size")]
        [Tooltip("Fraction of screen height the indicator should occupy.")]
        [SerializeField]
        private float _screenHeightFraction = 0.1f;

        [Tooltip("Small offset along view direction to avoid z-fighting with surfaces.")]
        [SerializeField]
        private float _surfaceOffset = 0.002f;

        [Header("Pulse Settings (Rings)")]
        [SerializeField]
        private float _pulseSpeed = 2f;

        [Header("Off-Axis Settings")]
        [Tooltip("If the angle from camera forward to the hitpoint exceeds this, show arrow at screen center.")]
        [SerializeField]
        private float _offAxisAngleThreshold = 30f;

        [Tooltip("Distance in front of the camera for the center arrow.")]
        [SerializeField]
        private float _centerDepth = 2f;

        private MaterialPropertyBlock _mpb;

        private static readonly int _pulseId = Shader.PropertyToID("_Pulse");
        private static readonly int _modeId = Shader.PropertyToID("_Mode");

        private bool _hasTarget;

        [Header("Cesium")]
        [SerializeField]
        private CesiumGeoreference _georeference;

        private CesiumGlobeAnchor _globeAnchor;

        private Vector3 _targetWorldPosition; // Fallback for non-Cesium scenes
        private double3 _targetEcef; // Stable Earth-fixed position for Cesium

        void Awake()
        {
            if (Cam == null)
            {
                Cam = Camera.main;
            }

            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<Renderer>();
            }

            _mpb = new MaterialPropertyBlock();

            if (_georeference == null)
            {
                _georeference = GetComponentInParent<CesiumGeoreference>();
            }

            _globeAnchor = GetComponent<CesiumGlobeAnchor>();
            if (_globeAnchor != null)
            {
                // We manage the transform via CesiumGeoreference; avoid double-transforming
                _globeAnchor.enabled = false;
            }
        }

        void OnDisable()
        {
            _hasTarget = false;
        }

        /// <summary>
        /// Sets the world-space target that the indicator represents.
        /// The visual will decide whether to show rings at the hitpoint
        /// or an arrow at screen center pointing toward this target.
        /// </summary>
        public void SetTargetPosition(Vector3 worldPosition)
        {
            if (_georeference != null)
            {
                double3 unityPos = new double3(worldPosition.x, worldPosition.y, worldPosition.z);
                _targetEcef = _georeference.TransformUnityPositionToEarthCenteredEarthFixed(unityPos);
            }
            else
            {
                _targetWorldPosition = worldPosition;
            }
            _hasTarget = true;
        }

        void LateUpdate()
        {
            if (Cam == null || _renderer == null || !_hasTarget)
            {
                return;
            }
            Transform camT = Cam.transform;
            Vector3 targetWorldPos;
            if (_georeference != null)
            {
                double3 unityPos =
                    _georeference.TransformEarthCenteredEarthFixedPositionToUnity(_targetEcef);
                targetWorldPos = new Vector3(
                    (float)unityPos.x,
                    (float)unityPos.y,
                    (float)unityPos.z
                );
            }
            else
            {
                targetWorldPos = _targetWorldPosition;
            }
            Vector3 toTarget = targetWorldPos - camT.position;
            float distance = toTarget.magnitude;

            if (distance <= 0.0001f)
            {
                return;
            }
            Vector3 dirToTarget = toTarget / distance;
            float angle = Vector3.Angle(camT.forward, dirToTarget);
            bool offAxis = angle > _offAxisAngleThreshold;
            _renderer.GetPropertyBlock(_mpb);
            if (offAxis)
            {
                UpdateAsArrow(camT, dirToTarget, distance);
            }
            else
            {
                UpdateAsRings(camT, targetWorldPos, dirToTarget, distance);
            }
            _renderer.SetPropertyBlock(_mpb);
        }

        private void UpdateAsRings(Transform camT, Vector3 targetWorldPos, Vector3 dirToTarget, float distance)
        {
            Transform t = transform;
            // Place the indicator at the stabilized target position
            t.position = targetWorldPos + dirToTarget * _surfaceOffset;
            // Billboard toward camera (world position remains earth-fixed)
            t.rotation = Quaternion.LookRotation(-camT.forward, camT.up);
            UpdateScale(distance);
            float pulse = Time.time * _pulseSpeed;
            _mpb.SetFloat(_pulseId, pulse);
            _mpb.SetFloat(_modeId, 0f); // Mode 0 = rings
        }

        private void UpdateAsArrow(Transform camT, Vector3 dirToTarget, float distance)
        {
            Transform t = transform;
            // Place the quad at the center of the screen in front of the camera
            float depth = Mathf.Max(_centerDepth, 0.01f);
            t.position = camT.position + camT.forward * depth;
            // Base orientation: facing the camera
            Quaternion baseRot = Quaternion.LookRotation(camT.forward, camT.up);
            // Compute 2D direction in camera space to rotate the arrow in-screen
            Vector3 dirCam = camT.InverseTransformDirection(dirToTarget);
            Vector2 dir2D = new Vector2(dirCam.x, dirCam.y);
            if (dir2D.sqrMagnitude > 0.0001f)
            {
                dir2D.Normalize();
                // Arrow texture is assumed to point "up" in local space (0,1)
                float angleAroundZ = Mathf.Atan2(-dir2D.x, dir2D.y) * Mathf.Rad2Deg;
                t.rotation = baseRot * Quaternion.AngleAxis(angleAroundZ, Vector3.forward);
            }
            else
            {
                t.rotation = baseRot;
            }
            // Keep apparent size stable at chosen depth
            UpdateScale(depth);
            // Mode 1 = arrow
            _mpb.SetFloat(_modeId, 1f);
        }

        private void UpdateScale(float distance)
        {
            if (Cam == null)
            {
                return;
            }

            float fovRad = Cam.fieldOfView * Mathf.Deg2Rad;
            float viewHeightAtDistance = 2f * distance * Mathf.Tan(fovRad * 0.5f);
            float worldSize = viewHeightAtDistance * _screenHeightFraction;

            transform.localScale = new Vector3(worldSize, worldSize, worldSize);
        }
    }
}
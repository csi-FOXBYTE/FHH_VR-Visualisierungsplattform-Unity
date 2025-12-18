using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace FHH.Logic.VR
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(XRRayInteractor))]
    public class XRayWidthByDistance : MonoBehaviour
    {
        [Header("References")] public XRRayInteractor RayInteractor;
        public LineRenderer Line; 

        [Header("Widths")] public float NearWidth = 0.004f;
        public float MinFarWidth = 0.004f;
        public float MaxFarWidth = 0.02f;
        public float WidthPerMeter = 0.0035f;

        [Header("Smoothing")] [Tooltip("Higher = snappier. 0 = no update.")]
        public float LerpSpeed = 12f;

        [Header("Fallbacks")] [Tooltip("Used when no hit and RayInteractor has no max distance available.")]
        public float FallbackMaxDistance = 10f;

        private AnimationCurve _curve;
        private float _smoothedFarWidth;

        private void Reset()
        {
            RayInteractor = GetComponent<XRRayInteractor>();
            if (Line == null) Line = GetComponent<LineRenderer>();
            if (Line == null) Line = GetComponentInChildren<LineRenderer>();
        }

        private void Awake()
        {
            if (RayInteractor == null) RayInteractor = GetComponent<XRRayInteractor>();
            if (Line == null) Line = GetComponent<LineRenderer>();
            if (Line == null) Line = GetComponentInChildren<LineRenderer>();

            // Initialize a 2-key curve: (0, NearWidth) and (1, NearWidth)
            _curve = new AnimationCurve(
                new Keyframe(0f, NearWidth, 0f, 0f),
                new Keyframe(1f, NearWidth, 0f, 0f)
            );
            ApplyCurve(NearWidth);
            _smoothedFarWidth = NearWidth;
        }

        private void LateUpdate()
        {
            float rayLen = GetCurrentRayLength();
            float targetFar = Mathf.Clamp(MinFarWidth + rayLen * WidthPerMeter, MinFarWidth, MaxFarWidth);

            // Optional smoothing
            float dt = Time.deltaTime;
            _smoothedFarWidth = (LerpSpeed > 0f)
                ? Mathf.Lerp(_smoothedFarWidth, targetFar, 1f - Mathf.Exp(-LerpSpeed * dt))
                : targetFar;

            ApplyCurve(_smoothedFarWidth);
        }

        private float GetCurrentRayLength()
        {
            if (RayInteractor != null &&
                RayInteractor.TryGetHitInfo(out Vector3 hitPos, out _, out _, out bool valid) &&
                valid)
            {
                return Vector3.Distance(RayInteractor.transform.position, hitPos);
            }
            
            float maxDist = FallbackMaxDistance;
            try
            {
                maxDist = RayInteractor != null ? RayInteractor.maxRaycastDistance : FallbackMaxDistance;
            }
            catch
            {
                maxDist = FallbackMaxDistance;
            }
            return maxDist;
        }

        private void ApplyCurve(float farWidth)
        {
            var k0 = _curve.keys.Length > 0 ? _curve.keys[0] : new Keyframe(0f, NearWidth);
            var k1 = _curve.keys.Length > 1 ? _curve.keys[1] : new Keyframe(1f, farWidth);

            if (k0.time != 0f) k0.time = 0f;
            if (!Mathf.Approximately(k1.time, 1f)) k1.time = 1f;

            k0.value = NearWidth;
            k1.value = farWidth;

            // Zero tangents for a clean linear-ish transition (no overshoot).
            k0.inTangent = 0f;
            k0.outTangent = 0f;
            k1.inTangent = 0f;
            k1.outTangent = 0f;

            _curve.keys = new[] { k0, k1 };

            if (Line != null)
            {
                Line.widthCurve = _curve;
                // Ensure width multiplier is 1 so the curve values are used as-is.
                if (!Mathf.Approximately(Line.widthMultiplier, 1f)) Line.widthMultiplier = 1f;
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Unity.XR.CoreUtils;

namespace FHH.Logic.VR
{
    [DisallowMultipleComponent]
    public sealed class DistanceCapTargetFilter : XRBaseTargetFilter
    {
        [SerializeField] [Tooltip("Maximum interaction distance in meters. 0 disables the cap.")]
        private float _maxDistanceMeters = 100f;

        [SerializeField]
        [Tooltip(
            "Optional XR Origin; if null, found at runtime. Used to convert meters to world units when the rig is scaled.")]
        private XROrigin _xrOrigin;

        public float MaxDistanceMeters
        {
            get => _maxDistanceMeters;
            set => _maxDistanceMeters = Mathf.Max(0f, value);
        }

        public override void Link(IXRInteractor interactor)
        {
            if (_xrOrigin == null) _xrOrigin = FindFirstObjectByType<XROrigin>();
        }

        public override void Unlink(IXRInteractor interactor)
        {
        }

        public override void Process(IXRInteractor interactor, List<IXRInteractable> targets,
            List<IXRInteractable> results)
        {
            results.Clear();

            if (_maxDistanceMeters <= 0f || targets.Count == 0)
            {
                if (targets.Count > 0) results.AddRange(targets);
                return;
            }

            Vector3 origin = GetInteractorOrigin(interactor);
            float metersToWorld = GetMetersToWorldScale(interactor); // see B)
            float maxWorld = _maxDistanceMeters * metersToWorld;
            float maxSq = maxWorld * maxWorld;

            // Try to get the current 3D raycast hit from the interactor (teleport ray, UI ray, etc.)
            RaycastHit hit = default;
            var ray = interactor as XRRayInteractor;
            bool hasHit = ray != null && ray.TryGetCurrent3DRaycastHit(out hit);

            for (int i = 0; i < targets.Count; i++)
            {
                var t = targets[i];
                Vector3 testPoint;

                if (hasHit && hit.collider != null)
                {
                    // If this candidate owns the hit, use the actual hit point distance
                    var targetRoot = ((Component)t).transform;
                    if (hit.collider.transform.IsChildOf(targetRoot))
                    {
                        testPoint = hit.point;
                    }
                    else
                    {
                        // Otherwise approximate with closest point on any collider the target has
                        testPoint = ClosestPointOnTarget(targetRoot, origin);
                    }
                }
                else
                {
                    // Fallback for non-ray interactors / no hit this frame
                    var attach = t.GetAttachTransform(interactor);
                    testPoint = attach != null ? attach.position : ((Component)t).transform.position;
                }

                if ((testPoint - origin).sqrMagnitude <= maxSq)
                    results.Add(t);
            }
        }

        private static Vector3 ClosestPointOnTarget(Transform targetRoot, Vector3 from)
        {
            float best = float.PositiveInfinity;
            Vector3 bestPt = targetRoot.position;
            var cols = targetRoot.GetComponentsInChildren<Collider>();
            for (int i = 0; i < cols.Length; i++)
            {
                Vector3 p = cols[i].ClosestPoint(from);
                float d = (p - from).sqrMagnitude;
                if (d < best)
                {
                    best = d;
                    bestPt = p;
                }
            }

            return bestPt;
        }


        private float GetMetersToWorldScale(IXRInteractor interactor)
        {
            float scale = 1f;
            if (_xrOrigin != null) scale = _xrOrigin.transform.lossyScale.x;
            else if (interactor is Component c) scale = c.transform.lossyScale.x;
            if (scale <= 0f) scale = 1f;
            // 1 real meter corresponds to (1/scale) world units when the XR rig is scaled
            return 1f / scale;
        }



        private static Vector3 GetInteractorOrigin(IXRInteractor interactor)
        {
            if (interactor is IXRRayProvider rayProvider)
            {
                var tr = rayProvider.GetOrCreateRayOrigin();
                if (tr != null) return tr.position;
            }

            var comp = interactor as Component;
            return comp != null ? comp.transform.position : Vector3.zero;
        }
    }
}
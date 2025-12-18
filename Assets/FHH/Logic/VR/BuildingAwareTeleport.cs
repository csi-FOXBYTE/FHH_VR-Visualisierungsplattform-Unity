using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using Unity.XR.CoreUtils; 

namespace FHH.Logic.VR
{
    [DisallowMultipleComponent]
    public class BuildingAwareTeleport : MonoBehaviour
    {
        [Header("References")] public XRRayInteractor RayInteractor;
        public XRInteractorLineVisual LineVisual;
        public TeleportationProvider TeleportationProvider;

        [Header("Input")] public InputActionReference SelectAction;

        [Header("Layers")] public LayerMask TerrainMask;
        public LayerMask BuildingMask;

        [Header("Visuals")] public Gradient NormalValidGradient;
        public Gradient NormalInvalidGradient;
        public Gradient BuildingAimGradient;

        [Header("Placement")] [Tooltip("Distance from the wall when aiming.")]
        public float StandOff = 0.8f; // meters from wall

        [Tooltip("Extra height above ground when aiming.")]
        public float ExtraHeight = 0.0f; // extra height above ground

        [Tooltip("Maximum vertical probe distance for ground.")]
        public float DowncastMax = 10f; // vertical probe distance for ground

        [Header("Limits")]
        [Tooltip("Maximum allowed teleport distance from the player. 0 disables the limit.")]
        [Range(10f,1000f)]public float MaxTeleportDistance = 500f;

        [Header("Player Origin (optional)")]
        public Transform PlayerRoot; 

        private bool _overMaxDistance;
        private bool _aimingAtBuilding;
        private bool _aimingAtTeleportArea;
        private RaycastHit _lastBuildingHit;
        private Gradient _cachedValidGradient;
        private Gradient _cachedInvalidGradient;
        private bool _subscribed;

        private void Awake()
        {
            if (RayInteractor == null) RayInteractor = GetComponent<XRRayInteractor>();
            if (LineVisual == null) LineVisual = GetComponent<XRInteractorLineVisual>();
            if (TeleportationProvider == null)
                TeleportationProvider = FindFirstObjectByType<TeleportationProvider>(FindObjectsInactive.Exclude);

            if (PlayerRoot == null)
            {
                var xrOrigin = FindFirstObjectByType<XROrigin>(FindObjectsInactive.Exclude);
                if (xrOrigin != null)
                {
                    // Prefer floor offset object (rig base on ground) if available
                    PlayerRoot = xrOrigin.CameraFloorOffsetObject != null
                        ? xrOrigin.CameraFloorOffsetObject.transform
                        : xrOrigin.transform;
                }
                else if (TeleportationProvider != null)
                {
                    PlayerRoot = TeleportationProvider.transform; // last-resort fallback
                }
            }

            if (LineVisual != null)
            {
                _cachedValidGradient =
                    NormalValidGradient != null ? NormalValidGradient : LineVisual.validColorGradient;
                _cachedInvalidGradient = NormalInvalidGradient != null
                    ? NormalInvalidGradient
                    : LineVisual.invalidColorGradient;

                // Normalize to our cached baselines
                LineVisual.validColorGradient = _cachedValidGradient;
                LineVisual.invalidColorGradient = _cachedInvalidGradient;
            }
        }

        private void OnEnable()
        {
            if (SelectAction != null && SelectAction.action != null && !_subscribed)
            {
                SelectAction.action.Enable();
                SelectAction.action.canceled += OnSelectCanceled; 
                _subscribed = true;
            }
        }

        private void OnDisable()
        {
            if (_subscribed && SelectAction != null && SelectAction.action != null)
            {
                SelectAction.action.canceled -= OnSelectCanceled;
                _subscribed = false;
            }

            RestoreNormalGradients();
            if (TeleportationProvider != null) TeleportationProvider.enabled = true;
        }

        private void Update()
        {
            UpdateAimStateAndLineColor();
        }

        private void UpdateAimStateAndLineColor()
        {
            _aimingAtBuilding = false;
            _aimingAtTeleportArea = false;
            _overMaxDistance = false;

            if (RayInteractor == null || LineVisual == null) return;

            if (RayInteractor.TryGetCurrent3DRaycastHit(out var hit))
            {
                int layerBit = 1 << hit.collider.gameObject.layer;
                bool isBuilding = (BuildingMask.value & layerBit) != 0;

                var area = hit.collider.GetComponentInParent<TeleportationArea>();
                var anchor = hit.collider.GetComponentInParent<TeleportationAnchor>();
                _aimingAtTeleportArea = area != null || anchor != null;

                
                Vector3 candidate = isBuilding && !_aimingAtTeleportArea
                    ? ComputeLandingNearBuilding(hit)  // building target uses standoff + ground snap
                    : hit.point;  // area/ground: use hit point (anchor usually aligns here)

                Vector3 from = (PlayerRoot != null ? PlayerRoot.position : RayInteractor.transform.position);
                float planarDistance = Vector2.Distance(new Vector2(from.x, from.z), new Vector2(candidate.x, candidate.z));
                _overMaxDistance = MaxTeleportDistance > 0f && planarDistance > MaxTeleportDistance;

                ToggleTeleportationProvider(!_overMaxDistance);

                if (_overMaxDistance)
                {
                    ApplyOverDistanceGradient();
                    return;
                }

                if (isBuilding && !_aimingAtTeleportArea)
                {
                    _aimingAtBuilding = true;
                    _lastBuildingHit = hit; // only cache when actually aiming at a building
                    ApplyBuildingGradient();
                    return;
                }

                RestoreNormalGradients();
            }
            else
            {
                RestoreNormalGradients();
                ToggleTeleportationProvider(true); // allow normal teleports when not hitting anything
            }
        }

        private void ToggleTeleportationProvider(bool enabled)
        {
            if (TeleportationProvider != null && TeleportationProvider.enabled != enabled)
                TeleportationProvider.enabled = enabled;
        }

        private void ApplyBuildingGradient()
        {
            if (LineVisual == null || BuildingAimGradient == null) return;

            // When aiming at a building the line is typically "invalid".
            // Override BOTH gradients so the special color is visible regardless of validity.
            if (LineVisual.validColorGradient != BuildingAimGradient)
                LineVisual.validColorGradient = BuildingAimGradient;

            if (LineVisual.invalidColorGradient != BuildingAimGradient)
                LineVisual.invalidColorGradient = BuildingAimGradient;
        }

        private void ApplyOverDistanceGradient()
        {
            if (LineVisual == null || _cachedInvalidGradient == null) return;
            if (LineVisual.validColorGradient != _cachedInvalidGradient) LineVisual.validColorGradient = _cachedInvalidGradient;
            if (LineVisual.invalidColorGradient != _cachedInvalidGradient) LineVisual.invalidColorGradient = _cachedInvalidGradient;
        }

        private void RestoreNormalGradients()
        {
            if (LineVisual == null) return;

            if (_cachedValidGradient != null && LineVisual.validColorGradient != _cachedValidGradient)
                LineVisual.validColorGradient = _cachedValidGradient;

            if (_cachedInvalidGradient != null && LineVisual.invalidColorGradient != _cachedInvalidGradient)
                LineVisual.invalidColorGradient = _cachedInvalidGradient;
        }

        private void OnSelectCanceled(InputAction.CallbackContext _)
        {
            // Only run custom teleport if we were aiming at a building on release.
            if (_aimingAtBuilding)
                TryTeleport();
        }

        public void TryTeleport()
        {
            if (!_aimingAtBuilding || TeleportationProvider == null) return;
            if (_overMaxDistance) return;

            Vector3 target = ComputeLandingNearBuilding(_lastBuildingHit);
            var request = new TeleportRequest
            {
                destinationPosition = target,
                destinationRotation = Quaternion.identity
            };
            TeleportationProvider.QueueTeleportRequest(request);
        }

        private Vector3 ComputeLandingNearBuilding(RaycastHit buildingHit)
        {
            //Vector3 candidate = buildingHit.point + buildingHit.normal * StandOff; // using the normal can be problematic on corners

            Vector3 fromPlayer = (RayInteractor.transform.position - buildingHit.point).normalized;
            Vector3 candidate = buildingHit.point + fromPlayer * StandOff;

            // Snap vertically to terrain
            Vector3 probeStart = candidate + Vector3.up * (DowncastMax * 0.5f);
            if (Physics.Raycast(probeStart, Vector3.down, out var groundHit, DowncastMax, TerrainMask,
                    QueryTriggerInteraction.Ignore))
                candidate.y = groundHit.point.y + ExtraHeight;
            else
                candidate.y += ExtraHeight; // fallback if no ground found

            return candidate;
        }
    }
}
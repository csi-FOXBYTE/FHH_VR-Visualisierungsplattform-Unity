//using CesiumForUnity;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.XR.Interaction.Toolkit.Interactors;

//namespace FHH.Logic.Cesium
//{
//    [DisallowMultipleComponent]
//    public class CesiumBuildingMetadataPicker : MonoBehaviour
//    {
//        [Header("XR")] public XRRayInteractor RayInteractor;
//        public InputActionReference ClickAction;

//        [Header("Filtering")] public LayerMask BuildingMask;
//        public string FeatureIdLabel = ""; // optional: choose a labeled Feature ID set
//        public int FeatureIdSetIndex = 0; // fallback index if no label provided

//        [Header("Last Pick (read-only at runtime)")]
//        public GameObject LastHitObject;

//        public long LastFeatureId = -1;
//        public Dictionary<string, string> LastMetadataStrings = new();

//        private bool _subscribed;

//        private void Awake()
//        {
//            if (RayInteractor == null) RayInteractor = GetComponent<XRRayInteractor>();
//        }

//        private void OnEnable()
//        {
//            if (ClickAction != null && ClickAction.action != null && !_subscribed)
//            {
//                ClickAction.action.Enable();
//                ClickAction.action.performed += OnClickPerformed;
//                _subscribed = true;
//            }
//        }

//        private void OnDisable()
//        {
//            if (_subscribed && ClickAction != null && ClickAction.action != null)
//            {
//                ClickAction.action.performed -= OnClickPerformed;
//                _subscribed = false;
//            }
//        }

//        private void OnClickPerformed(InputAction.CallbackContext _)
//        {
//            Debug.Log("clicked");
//            if (RayInteractor == null) return;
//            if (!RayInteractor.TryGetCurrent3DRaycastHit(out var hit)) return;

//            if (!IsInMask(hit.collider.gameObject.layer, BuildingMask)) return;

//            // Expect Cesium components on the primitive/tile that was hit.
//            var primitive = hit.collider.GetComponentInParent<CesiumPrimitiveFeatures>();
//            var modelMeta = hit.collider.GetComponentInParent<CesiumModelMetadata>();
//            if (primitive == null || modelMeta == null || primitive.featureIdSets == null ||
//                modelMeta.propertyTables == null)
//                return;

//            int setIndex = ResolveFeatureIdSetIndex(primitive);
//            if (setIndex < 0 || setIndex >= primitive.featureIdSets.Length) return;

//            long featureId = primitive.GetFeatureIdFromRaycastHit(hit, setIndex);
//            if (featureId < 0) return;

//            long tableIndexLong = primitive.featureIdSets[setIndex].propertyTableIndex;
//            int tableIndex = (tableIndexLong >= 0 && tableIndexLong <= int.MaxValue) ? (int)tableIndexLong : -1;
//            if (tableIndex < 0 || tableIndex >= modelMeta.propertyTables.Length) return;

//            var table = modelMeta.propertyTables[tableIndex];
//            var rawValues = table.GetMetadataValuesForFeature(featureId); // name -> CesiumMetadataValue
//            var strings = CesiumMetadataValue.GetValuesAsStrings(rawValues); // name -> string

//            // Save results
//            LastHitObject = hit.collider.gameObject;
//            LastFeatureId = featureId;
//            LastMetadataStrings = strings;
//            // Optionally: Debug.Log(string.Join(", ", strings.Select(kv => $"{kv.Key}={kv.Value}")));
//            Debug.Log(string.Join(", ", strings.Select(kv => $"{kv.Key}={kv.Value}")));
//        }

//        private int ResolveFeatureIdSetIndex(CesiumPrimitiveFeatures primitive)
//        {
//            if (!string.IsNullOrEmpty(FeatureIdLabel) && primitive.featureIdSets != null)
//            {
//                for (int i = 0; i < primitive.featureIdSets.Length; i++)
//                {
//                    if (string.Equals(primitive.featureIdSets[i].label, FeatureIdLabel,
//                            StringComparison.OrdinalIgnoreCase))
//                        return i;
//                }
//            }

//            return Mathf.Clamp(FeatureIdSetIndex, 0, Mathf.Max(0, (primitive.featureIdSets?.Length ?? 1) - 1));
//        }

//        private bool IsInMask(int layer, LayerMask mask)
//        {
//            int layerBit = 1 << layer;
//            return (mask.value & layerBit) != 0;
//        }
//    }
//}
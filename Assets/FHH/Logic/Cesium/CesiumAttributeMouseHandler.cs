using CesiumForUnity;
using Cysharp.Threading.Tasks;
using FHH.UI;
using FHH.UI.Attribute;
using Foxbyte.Presentation;
using System.Collections.Generic;
using FHH.Logic.Models;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace FHH.Logic.Cesium
{
    public class CesiumAttributeMouseHandler : MonoBehaviour
    {
        public int CityLayer;
        [SerializeField] private InputActionAsset InputAsset;
        private InputAction _attributeAction;
        private XROrigin _xrOrigin;

        void Start()
        {
            _xrOrigin = GameObject.FindFirstObjectByType<XROrigin>();
            if (_xrOrigin == null)
            {
                Debug.LogError("XROrigin not found via ServiceLocator.");
            }
        }

        void OnEnable()
        {
            _attributeAction = InputAsset.FindAction("Player/Attribute", true);
            _attributeAction.performed += OnAttributePerformed;
            _attributeAction.Enable();
        }

        void OnDisable()
        {
            _attributeAction.performed -= OnAttributePerformed;
            _attributeAction.Disable();
        }

        private void OnAttributePerformed(InputAction.CallbackContext ctx)
        {
            Ray ray = _xrOrigin.Camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out var hit, 200f, 1 << CityLayer, QueryTriggerInteraction.Ignore))
            {
                var hitTransform = hit.collider.transform;
                Dictionary<string, string> buildingMetadata;
                var projectModelAttributes = hitTransform.GetComponentInParent<ProjectModelAttributes>();
                // custom model attributes
                if (projectModelAttributes != null)
                {
                    var sourceAttributes = projectModelAttributes.Attributes;
                    buildingMetadata = sourceAttributes != null
                        ? new Dictionary<string, string>(sourceAttributes)
                        : new Dictionary<string, string>();
                }
                else
                {
                    // Cesium tileset attributes
                    var cesiumModelMetaData = hitTransform.GetComponentInParent<CesiumModelMetadata>();
                    if (cesiumModelMetaData == null)
                    {
                        Debug.LogWarning("No attribute provider (ProjectModelAttributes or CesiumModelMetadata) found on selected object.");
                        return;
                    }

                    buildingMetadata = BuildBuildingMetadataDictionary(cesiumModelMetaData);
                }
                ShowAttributeAsync(buildingMetadata).Forget();
            }
        }

        private Dictionary<string, string> BuildBuildingMetadataDictionary(CesiumModelMetadata modelMetadata)
        {
            var result = new Dictionary<string, string>();
            if (modelMetadata.propertyTables == null || modelMetadata.propertyTables.Length == 0)
            {
                return result;
            }
            for (var tableIndex = 0; tableIndex < modelMetadata.propertyTables.Length; tableIndex++)
            {
                var propertyTable = modelMetadata.propertyTables[tableIndex];
                if (propertyTable == null || propertyTable.count <= 0)
                {
                    continue;
                }
                var rowCount = propertyTable.count;
                for (long featureId = 0; featureId < rowCount; featureId++)
                {
                    var valuesForFeature = propertyTable.GetMetadataValuesForFeature(featureId);
                    if (valuesForFeature == null || valuesForFeature.Count == 0)
                    {
                        continue;
                    }
                    var stringValues = CesiumMetadataValue.GetValuesAsStrings(valuesForFeature);
                    if (!stringValues.TryGetValue("citygrid_UnitID", out var unitId) ||
                        string.IsNullOrEmpty(unitId) ||
                        unitId == "-")
                    {
                        continue; // skip this feature row if ID is empty or "-"
                    }
                    foreach (var kvp in stringValues)
                    {
                        var key = kvp.Key; 
                        var value = kvp.Value ?? string.Empty;
                        // skip any entry where the value is empty or "-"
                        if (string.IsNullOrEmpty(value) || value == "-")
                        {
                            continue;
                        }
                        // "last one wins" if multiple rows/tables define same key
                        result[key] = value;
                    }
                }
            }
            return result;
        }

        private async UniTask ShowAttributeAsync(Dictionary<string,string> metaData)
        {
            var uiManager = ServiceLocator.GetService<UIManager>();
            if (uiManager.IsShown<AttributePresenter>())
            {
                await uiManager.HideAsync<AttributePresenter>();
                return;
            }
            await uiManager.HideRegionAsync(UIProjectContext.UIRegion.Content1);
            
            var options = new WindowOptions
            {
                StyleSheet = Resources.Load<StyleSheet>("Attribute"),
                CenterScreen = false,
                Region = UIProjectContext.UIRegion.Content1
            };

            uiManager.ShowWindowAsync<AttributePresenter, AttributeView, AttributeModel>(
                model: null,
                initData: metaData,
                options: options).Forget();
            
            await UniTask.Yield();
        }
    }
}
using CesiumForUnity;
using Cysharp.Threading.Tasks;
using FHH.UI;
using FHH.UI.Attribute;
using Foxbyte.Presentation;
using System.Collections.Generic;
using FHH.Logic.Models;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace FHH.Logic.Cesium
{
    /// <summary>
    /// Makes buildings in a Cesium 3D TileSet interactable using XR Interaction Toolkit.
    /// </summary>
    [RequireComponent(typeof(Cesium3DTileset))]
    public class CesiumBuildingInteractable : MonoBehaviour
    {
        private Cesium3DTileset _tileSet;
        [SerializeField] private XRInteractionManager _interactionManager;
        [SerializeField] private InteractionLayerMask _buildingSelectLayerMask;
        
        void Awake()
        {
            _tileSet = GetComponent<Cesium3DTileset>();
            _tileSet.OnTileGameObjectCreated += AddComponents;
            if (_interactionManager == null)
            {
                Debug.LogError("Assign a XRInteractionManager.");
            }

            if (_buildingSelectLayerMask == 0)
            {
                Debug.LogError("Assign a buildingSelect layer mask.");
            }
        }

        void OnDestroy() => _tileSet.OnTileGameObjectCreated -= AddComponents;

        private void AddComponents(GameObject tileGo)
        {
            // xr simple interactable
            var interactable = tileGo.AddComponent<XRSimpleInteractable>();
            interactable.interactionManager = _interactionManager;
            interactable.interactionLayers = _buildingSelectLayerMask;
            interactable.activated.AddListener(args =>
            {
                Debug.Log($"Selected building {tileGo.name}");
                Dictionary<string, string> buildingMetadata;
                // Custom model (ProjectModelRef) attributes
                var projectModelAttributes = tileGo.GetComponentInParent<ProjectModelAttributes>();
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
                    var cesiumModelMetaData = tileGo.GetComponent<CesiumModelMetadata>();
                    if (cesiumModelMetaData == null)
                    {
                        Debug.LogWarning("No CesiumModelMetadata or ProjectModelAttributes found on selected building.");
                        return;
                    }
                    buildingMetadata = BuildBuildingMetadataDictionary(cesiumModelMetaData);
                }
                ShowAttributeAsync(buildingMetadata).Forget();
            });
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
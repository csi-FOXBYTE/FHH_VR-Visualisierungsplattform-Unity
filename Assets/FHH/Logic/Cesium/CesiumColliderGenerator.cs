using CesiumForUnity;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace FHH.Logic.Cesium
{
    /// <summary>
    /// Generates colliders for the Cesium 3D TileSet.
    /// Turns off the default physics mesh generation.
    /// Optionally turns off shadow casting for all renderers.
    /// </summary>
    [RequireComponent(typeof(Cesium3DTileset))]
    public class CesiumColliderGenerator : MonoBehaviour
    {
        [SerializeField] private Boolean _castShadows;
        public enum Shape { Mesh, ConvexMesh, Box }
        public Shape ColliderShape = Shape.Box;  
        private Cesium3DTileset _tileSet;
        private MeshColliderCookingOptions _cookingOptions = MeshColliderCookingOptions.None;
        [SerializeField] private XRInteractionManager _interactionManager;
        [SerializeField] private TeleportationProvider _teleportationProvider;
        [SerializeField] private InteractionLayerMask _teleportationInteractionLayerMask;
        [SerializeField] private InteractionLayerMask _buildingSelectLayerMask;
        

        void Awake()
        {
            _tileSet = GetComponent<Cesium3DTileset>();
            _tileSet.createPhysicsMeshes = false;  // make sure it's off
            _tileSet.OnTileGameObjectCreated += AddComponents;
            _cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.UseFastMidphase;
            if (_interactionManager == null)
            {
                Debug.LogError("Assign a XRInteractionManager to the CesiumColliderGenerator.");
            }
            if (_teleportationProvider == null)
            {
                Debug.LogError("Assign a TeleportationProvider to the CesiumColliderGenerator.");
            }
            if (_teleportationInteractionLayerMask == 0)
            {
                Debug.LogError("Assign a InteractionLayerMask to the CesiumColliderGenerator.");
            }
        }

        void OnDestroy() => _tileSet.OnTileGameObjectCreated -= AddComponents;

        private void AddComponents(GameObject tileGo)
        {
            // shadows
            if (!_castShadows)
            {
                foreach (var r in tileGo.GetComponentsInChildren<Renderer>())
                {
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }

            // collider
            foreach (var mf in tileGo.GetComponentsInChildren<MeshFilter>())
            {
                switch (ColliderShape)
                {
                    case Shape.Box:
                        var box = mf.gameObject.AddComponent<BoxCollider>();
                        break;

                    case Shape.ConvexMesh:
                        var convex = mf.gameObject.AddComponent<MeshCollider>();
                        convex.sharedMesh = mf.sharedMesh;
                        convex.convex = true;
                        convex.cookingOptions = _cookingOptions;
                        break;

                    default:
                        var mesh = mf.gameObject.AddComponent<MeshCollider>();
                        mesh.cookingOptions = _cookingOptions;
                        mesh.sharedMesh = mf.sharedMesh;
                        break;
                }
            }

            // teleportation area
            var area = tileGo.AddComponent<TeleportationArea>();
            area.interactionManager = _interactionManager;
            area.teleportationProvider = _teleportationProvider;
            area.interactionLayers = _teleportationInteractionLayerMask;  //LayerMask.GetMask("Teleport");
            area.teleportTrigger = TeleportationArea.TeleportTrigger.OnActivated;
        
        }
    }
}
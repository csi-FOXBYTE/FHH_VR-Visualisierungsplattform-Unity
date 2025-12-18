using UnityEngine;
using CesiumForUnity;

namespace FHH.Logic.Cesium
{
    /// <summary>
    /// Updates the layer of all GameObjects created by the Cesium3DTileset to match the layer of this GameObject.
    /// Used for collision detection and raycasting.
    /// </summary>
    [RequireComponent(typeof(Cesium3DTileset))]
    public class CesiumLayerPropagator : MonoBehaviour
    {
        void Awake()
        {
            var tileset = GetComponent<Cesium3DTileset>();
            tileset.OnTileGameObjectCreated += go => go.layer = gameObject.layer;
        }
    }
}
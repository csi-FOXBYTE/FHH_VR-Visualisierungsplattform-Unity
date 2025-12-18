using CesiumForUnity;
using UnityEngine;

namespace FHH.Logic.Cesium
{
    public static class CesiumTilesetUtilities
    {
        /// <summary>
        /// Sets maximumScreenSpaceError on all active Cesium3DTileset components.
        /// </summary>
        /// <param name="maxError">Desired maximum screen space error.</param>
        public static void SetMaximumScreenSpaceErrorOnAllTilesets(float maxError)
        {
            Cesium3DTileset[] tilesets = Object.FindObjectsByType<Cesium3DTileset>(FindObjectsSortMode.None);

            foreach (Cesium3DTileset tileset in tilesets)
            {
                if (tileset != null && tileset.enabled && tileset.gameObject.activeInHierarchy)
                {
                    tileset.maximumScreenSpaceError = maxError;
                }
            }
        }
    }
}
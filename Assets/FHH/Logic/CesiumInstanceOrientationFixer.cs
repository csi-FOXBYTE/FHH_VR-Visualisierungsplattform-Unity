using UnityEngine;
using CesiumForUnity;

namespace FHH.Logic
{
    /// <summary>
    /// This component fixes the orientation of tiles created by Cesium3DTileset.
    /// It corrects a mistake in Cesium for Unity where instances have a wrong rotation
    /// because the conversion of vector3 to quaternion is not done correctly.
    /// Should be fixed in future releases of Cesium for Unity.
    /// </summary>
    [DisallowMultipleComponent]
    public class CesiumInstanceOrientationFixer : MonoBehaviour
    {
        [Tooltip("Euler angles (degrees) to apply to each mesh under the tile.")]
        public Vector3 EulerCorrection = new Vector3(0f, 0f, -90f);

        private Cesium3DTileset _tileset;
        private Quaternion _correctionQuat;

        void Awake()
        {
            _tileset = GetComponent<Cesium3DTileset>();
            _correctionQuat = Quaternion.Euler(this.EulerCorrection);
        }

        void OnEnable()
        {
            _tileset.OnTileGameObjectCreated += RotateTileMeshes;
        }

        void OnDisable()
        {
            _tileset.OnTileGameObjectCreated -= RotateTileMeshes;
        }

        private void RotateTileMeshes(GameObject tileGo)
        {
            var globeAnchors = tileGo.GetComponentsInChildren<CesiumGlobeAnchor>();
            if (globeAnchors == null || globeAnchors.Length == 0)
                return;

            foreach (var globeAnchor in globeAnchors)
            {
                //add 90° Euler X rotation to the globeAnchor.rotationEastUpNorth using the EulerCorrection variable
                globeAnchor.rotationEastUpNorth = _correctionQuat * globeAnchor.rotationEastUpNorth;
            }
        }
    }
}
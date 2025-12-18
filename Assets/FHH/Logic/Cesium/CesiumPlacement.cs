using CesiumForUnity;
using Unity.Mathematics;
using UnityEngine;

namespace FHH.Logic.Cesium
{
    /// <summary>
    /// Places a GameObject using CesiumGlobeAnchor with an ECEF position (meters) and EUN rotation.
    /// Also ensures CesiumOriginShift is present for precision stability.
    /// Uses reflection to avoid a hard compile-time dependency on Cesium types.
    /// </summary>
    public static class CesiumPlacement
    {

        private const double _a = 6378137.0;
        private const double _f = 1.0 / 298.257223563;
        private static readonly double _b  = _a * (1.0 - _f);
        private static readonly double _e2 = 1.0 - (_b * _b) / (_a * _a);
        private static readonly double _ep2 = (_a * _a - _b * _b) / (_b * _b);


        public static void AnchorAtEcef(
            GameObject go,
            FHH.Logic.Models.Vector3d ecef,
            Quaternion serverRotation,
            Vector3 scaleEun)
        {
            if (go == null || ecef == null) return;

            var anchor = go.GetComponent<CesiumGlobeAnchor>();
            if (anchor == null) anchor = go.AddComponent<CesiumGlobeAnchor>();

            var originShift = go.GetComponent<CesiumOriginShift>();
            if (originShift == null) originShift = go.AddComponent<CesiumOriginShift>();

            anchor.positionGlobeFixed = new double3(ecef.X, ecef.Y, ecef.Z);
            anchor.scaleEastUpNorth   = new double3(scaleEun.x, scaleEun.y, scaleEun.z);
            //const QuatLayout layout = QuatLayout.XYZW;
            //Quaternion qServerEcef =
            //    (layout == QuatLayout.WXYZ)
            //        ? new Quaternion(serverRotation.y, serverRotation.z, serverRotation.w, serverRotation.x) // (w,x,y,z) -> (x,y,z,w)
            //        : serverRotation;
            Quaternion qServerEcef = serverRotation;
            
            anchor.rotationGlobeFixed = Normalize(qServerEcef);
           
        }

        private static Quaternion ToUnity(quaternion q) => new Quaternion(q.value.x, q.value.y, q.value.z, q.value.w);

        private static Quaternion Normalize(Quaternion q)
        {
            float m = Mathf.Sqrt(q.x*q.x + q.y*q.y + q.z*q.z + q.w*q.w);
            return m > 1e-6f ? new Quaternion(q.x/m, q.y/m, q.z/m, q.w/m) : Quaternion.identity;
        }

        private enum QuatLayout
        {
            XYZW,
            WXYZ
        } // how components are serialized
    }
}
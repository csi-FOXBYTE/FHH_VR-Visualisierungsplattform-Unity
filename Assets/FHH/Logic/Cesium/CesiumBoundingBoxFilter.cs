#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// Suppresses Cesium's log messages about bounding volumes that do not include all of their content.
/// The 3D tiles coming from the backend are optimized for performance so this warning is not relevant.
/// </summary>
internal static class CesiumBoundingBoxFilter
{
    private const string FilterToken = "bounding volume that does not include all of its content";

    private static readonly ILogHandler _default = Debug.unityLogger.logHandler;

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Install()
    {
        Debug.unityLogger.logHandler = new Handler();
        //Debug.Log("CesiumBoundingBoxFilter installed. Suppressing bounding volume warnings.");
    }

    private sealed class Handler : ILogHandler
    {
        //[HideInCallstack]
        public void LogFormat(LogType type, Object ctx, string fmt, params object[] args)
        {
            if (type == LogType.Log && ShouldFilter(args)) return; // swallow
            _default.LogFormat(type, ctx, fmt, args); // forward
        }

        //[HideInCallstack]
        public void LogException(System.Exception ex, Object ctx) =>
            _default.LogException(ex, ctx);

        private static bool ShouldFilter(object[] args)
        {
            // Cesium sometimes logs with fmt = "{0}", real text in args[0]
            //if (fmt?.Contains(FilterToken) == true) return true;
            if (args is { Length: > 0 } && args[0]?.ToString().Contains(FilterToken) == true)
                return true;
            return false;
        }
    }
}



// The log message for reference:
//[warning] [TilesetContentManager.cpp:524] Tile has a bounding volume that does not include all of its content,
//so culling and raster overlays may be incorrect: .../hamburg/terrain/0/0/0.terrain upsampled L1-X0-Y1
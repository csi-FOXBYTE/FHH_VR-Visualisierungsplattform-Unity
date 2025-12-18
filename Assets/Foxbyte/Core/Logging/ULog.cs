using UnityEngine;

namespace Foxbyte.Core
{
    public static class ULog
    {
        //[HideInCallstack]
        public static void Info(string message)
        {
            Debug.Log($"[Info] {message}");
        }

        //[HideInCallstack]
        public static void Error(string message)
        {
            Debug.LogError($"[Error] {message}");
        }

        //[HideInCallstack]
        public static void Warning(string message)
        {
            Debug.LogWarning($"[Warning] {message}");
        }
    }
}
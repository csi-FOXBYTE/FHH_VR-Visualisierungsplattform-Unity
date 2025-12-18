using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Foxbyte.Presentation.Rendering.FadeOverlayFeatureSRP
{
    [DisallowMultipleComponent]
    public sealed class FadeOverlayControllerSRP : MonoBehaviour
    {
        [Header("Target Render Feature")]
        public FadeOverlayFeatureSRP FeaturePerformance;
        public FadeOverlayFeatureSRP FeatureBeauty;

        [Header("Options")]
        public bool UseUnscaledTime = false;

        public bool IsOpaqueAtStart = true;

        private CancellationTokenSource _cts;

        void Awake()
        {
            if (IsOpaqueAtStart)
            {
                FeaturePerformance.Intensity = 1f;
                FeatureBeauty.Intensity = 1f;
            }
        }

        public async UniTask FadeAsync(float targetFade, float duration, CancellationTokenSource externalToken = default)
        {
            if (FeaturePerformance == null || FeatureBeauty == null)
            {
                Debug.LogError($"{nameof(FadeOverlayControllerSRP)}: Feature reference is null.");
                return;
            }

            targetFade = Mathf.Clamp01(targetFade);
            if (duration <= 0f)
            {
                FeaturePerformance.Intensity = targetFade;
                FeatureBeauty.Intensity = targetFade;
                return;
            }

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            CancellationToken token = _cts.Token;
            CancellationToken linkedToken = token;
            CancellationTokenSource linkedCts = null;

            try
            {
                if (externalToken != null && externalToken.Token.CanBeCanceled)
                {
                    linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, externalToken.Token);
                    linkedToken = linkedCts.Token;
                }

                float start = Mathf.Clamp01(FeaturePerformance.Intensity);
                float time = 0f;

                while (time < duration)
                {
                    linkedToken.ThrowIfCancellationRequested();

                    time += UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    float t = Mathf.Clamp01(time / duration);
                    float eased = EaseInOutCubic(t);

                    FeaturePerformance.Intensity = Mathf.LerpUnclamped(start, targetFade, eased);
                    FeatureBeauty.Intensity = Mathf.LerpUnclamped(start, targetFade, eased);

                    await UniTask.Yield(PlayerLoopTiming.Update, linkedToken);
                }

                FeaturePerformance.Intensity = targetFade;
                FeatureBeauty.Intensity = targetFade;
            }
            catch (OperationCanceledException)
            {
                // Swallow cancellation to keep logs clean.
            }
            finally
            {
                linkedCts?.Dispose();
            }
        }

        private static float EaseInOutCubic(float t)
        {
            return t < 0.5f
                ? 4f * t * t * t
                : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;
        }

        private void OnDisable()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }
}
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering.Universal;

namespace Foxbyte.Presentation.Rendering.FadeOverlayFeature
{
    public class FadeOverlayController : MonoBehaviour
    {
        [Header("Overlay")] public Color BaseColor = Color.black;
        [Range(0f, 1f)] public float Fade = 1f;

        [Header("Injection")] public RenderPassEvent InjectionPoint = RenderPassEvent.BeforeRenderingTransparents;

        [Header("Camera Filtering")]
        public FadeOverlayFeature.CameraFilter CameraScope = FadeOverlayFeature.CameraFilter.AllCameras;

        public string TargetCameraTag = "MainCamera";

        [Header("Editor Visibility")] public bool EnabledInEditMode = true;

        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            Apply();
        }

        private void OnValidate()
        {
            Apply();
        }

        private void Update()
        {
            Apply();
        } // keep scene view responsive while authoring

        private void Apply()
        {
            var f = FadeOverlayFeature.Instance;
            if (f == null) return;

            f.SetColor(BaseColor);
            f.SetFade(Fade);
            f.SetInjectionPoint(InjectionPoint);
            f.SetCameraFilter(CameraScope);
            f.SetTargetCameraTag(TargetCameraTag);
            f.SetEnabledInEditMode(EnabledInEditMode);
        }

        public void SetColor(Color color)
        {
            BaseColor = color;
            Apply();
        }

        public void SetFade(float value)
        {
            Fade = Mathf.Clamp01(value);
            Apply();
        }

        public void SetInjection(RenderPassEvent evt)
        {
            InjectionPoint = evt;
            Apply();
        }

        public void SetEditorEnabled(bool enabled)
        {
            EnabledInEditMode = enabled;
            Apply();
        }

        public void SetCameraFilter(FadeOverlayFeature.CameraFilter scope, string tagIfAny = null)
        {
            CameraScope = scope;
            if (!string.IsNullOrEmpty(tagIfAny)) TargetCameraTag = tagIfAny;
            Apply();
        }

        // Async fade using UniTask
        public async UniTask FadeToAsync(float targetFade, float duration, AnimationCurve curve = null,
            CancellationToken externalToken = default)
        {
            CancelAnimation();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);

            float start = Fade;
            float t = 0f;
            duration = Mathf.Max(0.0001f, duration);

            while (t < duration && !_cts.IsCancellationRequested)
            {
                t += Time.deltaTime;
                float x = Mathf.Clamp01(t / duration);
                float k = (curve != null) ? curve.Evaluate(x) : x;
                SetFade(Mathf.Lerp(start, Mathf.Clamp01(targetFade), k));
                await UniTask.Yield(PlayerLoopTiming.Update, _cts.Token);
            }

            if (!_cts.IsCancellationRequested) SetFade(Mathf.Clamp01(targetFade));
        }

        public void CancelAnimation()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}
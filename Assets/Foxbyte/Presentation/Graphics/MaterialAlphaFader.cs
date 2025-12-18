using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Foxbyte.Presentation.Graphics
{
    [RequireComponent(typeof(Renderer))]
    public sealed class MaterialAlphaFader : MonoBehaviour
    {
        public Renderer TargetRenderer;
        public int MaterialIndex = 0;
        public bool IgnoreTimeScale = true;
        public float DefaultDuration = 0.5f;

        private static readonly int _BaseColorID = Shader.PropertyToID("_BaseColor");
        private static readonly int _ColorID = Shader.PropertyToID("_Color");

        private Renderer _renderer;
        private Material _matInstance;
        private int _colorPropId = -1;
        private bool _initialized;

        void Awake()
        {
            EnsureInit();
        }

        public async UniTask FadeInAsync(float duration = -1f, CancellationToken ct = default)
        {
            EnsureInit();
            if (!_initialized) return;
            if (duration < 0f) duration = DefaultDuration;
            await FadeToAsync(1f, duration, ct);
        }

        public async UniTask FadeOutAsync(float duration = -1f, CancellationToken ct = default)
        {
            EnsureInit();
            if (!_initialized) return;
            if (duration < 0f) duration = DefaultDuration;
            await FadeToAsync(0f, duration, ct);
        }

        public async UniTask FadeToAsync(float targetAlpha, float duration, CancellationToken ct = default)
        {
            EnsureInit();
            if (!_initialized) return;

            float startAlpha = GetAlphaFromMaterial();
            if (duration <= 0f)
            {
                SetAlphaOnMaterial(targetAlpha);
                return;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                ct.ThrowIfCancellationRequested();
                elapsed += IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                SetAlphaOnMaterial(Mathf.Lerp(startAlpha, targetAlpha, t));
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            SetAlphaOnMaterial(targetAlpha);
        }

        private void EnsureInit()
        {
            if (_initialized) return;

            _renderer = TargetRenderer != null ? TargetRenderer : GetComponent<Renderer>();
            if (_renderer == null)
            {
                Debug.LogError($"{nameof(MaterialAlphaFader)}: No Renderer found.");
                return;
            }
            var mats = _renderer.materials;
            if (MaterialIndex < 0 || MaterialIndex >= mats.Length)
            {
                Debug.LogError($"{nameof(MaterialAlphaFader)}: MaterialIndex {MaterialIndex} is out of range.");
                return;
            }

            _matInstance = mats[MaterialIndex];

            // Detect color property
            if (_matInstance.HasProperty(_BaseColorID)) _colorPropId = _BaseColorID;
            else if (_matInstance.HasProperty(_ColorID)) _colorPropId = _ColorID;
            else throw new InvalidOperationException("Shader has no _BaseColor or _Color property.");

            // Warn if still opaque; transparent queue is >= 3000
            if (_matInstance.renderQueue < 3000)
            {
                Debug.LogWarning($"{nameof(MaterialAlphaFader)}: Material is Opaque. " +
                                 "Set URP Unlit Surface Type = Transparent so alpha actually blends.");
            }

            _initialized = true;
        }

        private float GetAlphaFromMaterial()
        {
            Color c = _matInstance.GetColor(_colorPropId);
            return c.a;
        }

        private void SetAlphaOnMaterial(float a)
        {
            Color c = _matInstance.GetColor(_colorPropId);
            c.a = a;
            _matInstance.SetColor(_colorPropId, c);
        }
    }
}
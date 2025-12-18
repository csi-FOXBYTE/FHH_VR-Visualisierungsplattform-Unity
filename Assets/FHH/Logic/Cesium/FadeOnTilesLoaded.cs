using System;
using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;

namespace FHH.Logic.Cesium
{
    public class FadeOnTilesLoaded : MonoBehaviour
    {
        [SerializeField] private CesiumTilesLoadingProgressProvider _tilesLoadingProgressProvider;
        [SerializeField] [Range(0.1f, 5f)] private float _fadeDuration = 1.0f;
        [SerializeField] [Range(0.1f, 5f)] private float _delayBeforeFade = 2f;
        [SerializeField] private UIDocument _uiDocument;

        // fade the current uitoolkit uidocument root visual element on tiles loaded

        void Awake()
        {
            if (_uiDocument != null)
            {
                var rootElement = _uiDocument.rootVisualElement;
                rootElement.style.opacity = 0;
            }
        }

        private void Start()
        {
            _tilesLoadingProgressProvider.TilesLoaded += HandleTilesLoaded;
            if (ServiceLocator.Instance.IsOffline)
            {
                HandleTilesLoaded();
            }
        }

        private void HandleTilesLoaded()
        {
            FadeAsync().Forget();
        }

        private async UniTask FadeAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeFade));
            if (_uiDocument == null) return;
            var rootElement = _uiDocument.rootVisualElement;
            float elapsedTime = 0f;
            float startOpacity = rootElement.resolvedStyle.opacity;
            float targetOpacity = 1f;
            while (elapsedTime < _fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newOpacity = Mathf.Lerp(startOpacity, targetOpacity, elapsedTime / _fadeDuration);
                rootElement.style.opacity = newOpacity;
                await UniTask.Yield();
            }

            rootElement.style.opacity = targetOpacity;
        }
    }
}
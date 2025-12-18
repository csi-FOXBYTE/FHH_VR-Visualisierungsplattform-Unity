using Cysharp.Threading.Tasks;
using FHH.Logic;
using FHH.Logic.Cesium;
using Foxbyte.Presentation.Extensions;
using Foxbyte.Presentation.Graphics;
using Foxbyte.Presentation.Rendering.FadeOverlayFeatureSRP;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.LoadingScreen
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private FadeOverlayControllerSRP _fadeOverlayController;
        [SerializeField] private UIDocument _uiDocumentWorldSpace;
        [SerializeField] private UIDocument _uiDocumentDesktop;
        [SerializeField] private GameObject _worldSpaceGo;
        [SerializeField] private GameObject _desktopGo;
        private CesiumTilesLoadingProgressProvider _tilesLoadingProgressProvider;
        [Range(0.01f, 4f)] public float FadeTime = 1f;
        [SerializeField] [Range(0.01f, 4f)] private float _secondsToWaitBeforeDeactivation = 1.5f;

        [Header("If RenderFeature incompatibility use fallback")]
        // fallback if RenderFeature has compatibility issues
        // activate UICam, UIBackground in hierarchy; add UICam to cam stack of main cam
        [SerializeField] private bool UseFallbackLoadingBackground = false;
        [SerializeField] private GameObject _uiBackgroundGO;
        private MaterialAlphaFader _matAlphaFader;
        
        [Header("Offline / timeout handling")]
        [SerializeField] [Range(0.5f, 60f)] private float _maxSecondsWithoutProgress = 10f;
        private bool _isOffline;
        private bool _deactivationRequested;
        private float _lastProgressTime;
        private float _lastProgressValue;

        void Awake()
        {
            _isOffline = ServiceLocator.Instance.IsOffline ||
                         Application.internetReachability == NetworkReachability.NotReachable;
            _lastProgressTime = Time.unscaledTime;
            _desktopGo.SetActive(true);
            _worldSpaceGo.SetActive(true);
            _uiDocumentDesktop.rootVisualElement.style.opacity = 0f;
            _uiDocumentWorldSpace.rootVisualElement.style.opacity = 0f;
            _tilesLoadingProgressProvider = FindFirstObjectByType<CesiumTilesLoadingProgressProvider>();
            if (_tilesLoadingProgressProvider == null)
            {
                Debug.LogError("CesiumTilesLoadingProgressProvider not found in the scene.");
                _isOffline = true; // treat missing provider as “no streaming info”
                //return;
            }
            else if (!_isOffline)
            {
                _tilesLoadingProgressProvider.TilesLoaded += OnTilesLoaded;
                _tilesLoadingProgressProvider.TilesLoadingProgressChanged += OnTilesLoadingProgressChanged;
            }
            else
            {
                Debug.Log("LoadingScreen running in OFFLINE mode; Cesium tiles will not gate UI.");
            }
           
            // fallback background setup
            if (!UseFallbackLoadingBackground) return;
            _matAlphaFader = _uiBackgroundGO.GetComponent<MaterialAlphaFader>();
            if (_matAlphaFader == null)
            {
                Debug.LogError("MaterialAlphaFader component not found on uiBackgroundGO.");
                return;
            }
            _matAlphaFader.FadeToAsync(1f, 0f).Forget();
        }

        void Start()
        {
            FadeInAsync().Forget();
        }
        
        private async UniTask FadeInAsync()
        {
            await UniTask.DelayFrame(1);
            await _uiDocumentDesktop.rootVisualElement.FadeToAsync(1f, 0.6f);
            await _uiDocumentWorldSpace.rootVisualElement.FadeToAsync(1f, 0.6f);
        }

        void Update()
        {
            if (_deactivationRequested)
            {
                return;
            }

            // Offline or no provider: simple timeout after initial delay
            if (_isOffline)
            {
                if (Time.unscaledTime - _lastProgressTime >= _secondsToWaitBeforeDeactivation)
                {
                    _deactivationRequested = true;
                    DeactivateAsync().Forget();
                }
                return;
            }

            // Online with provider, but stuck progress -> force close after maxSecondsWithoutProgress
            if (_tilesLoadingProgressProvider != null &&
                Time.unscaledTime - _lastProgressTime >= _maxSecondsWithoutProgress)
            {
                Debug.LogWarning("No tiles loading progress change for timeout duration. Forcing LoadingScreen deactivation.");
                _deactivationRequested = true;
                DeactivateAsync().Forget();
            }
        }
        
        private void OnDestroy()
        {
            if (_tilesLoadingProgressProvider != null)
            {
                _tilesLoadingProgressProvider.TilesLoaded -= OnTilesLoaded;
                _tilesLoadingProgressProvider.TilesLoadingProgressChanged -= OnTilesLoadingProgressChanged;
            }
        }

        private void OnTilesLoaded()
        {
            if (_deactivationRequested)
            {
                return;
            }
            _deactivationRequested = true;
            DeactivateAsync().Forget();
        }

        private async UniTaskVoid DeactivateAsync()
        {
            // delay fading because we reposition the xr rig and it will load new tiles at target location
            await UniTask.WaitForSeconds(_secondsToWaitBeforeDeactivation);
            await _uiDocumentWorldSpace.rootVisualElement.FadeToAsync(0f, FadeTime / 3f);
            if (!UseFallbackLoadingBackground)
            {
                await UniTask.WhenAll(
                    _uiDocumentDesktop.rootVisualElement.FadeToAsync(0f, FadeTime / 3f),
                    _fadeOverlayController.FadeAsync(0f, FadeTime / 3f)
                );
            }
            else
            {
                await UniTask.WhenAll(
                    _uiDocumentDesktop.rootVisualElement.FadeToAsync(0f, FadeTime / 3f),
                    _matAlphaFader.FadeOutAsync(FadeTime / 3f)
                );
            }
            await UniTask.NextFrame();
            gameObject.SetActive(false);
            if (UseFallbackLoadingBackground)
                _uiBackgroundGO.SetActive(false);
        }

        private void OnTilesLoadingProgressChanged(float progress)
        {
            _lastProgressTime = Time.unscaledTime;
            _lastProgressValue = progress;
            _uiDocumentWorldSpace.rootVisualElement.Q<ProgressBar>("LoadingProgressBar").value = progress;
            _uiDocumentDesktop.rootVisualElement.Q<ProgressBar>("LoadingProgressBar").value = progress;
        }
    }
}
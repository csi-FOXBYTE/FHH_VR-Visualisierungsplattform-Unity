using System;
using Cysharp.Threading.Tasks;
using Foxbyte.Presentation.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Intro
{
    public class IntroCodeBehind : MonoBehaviour
    {
        [SerializeField] [Range(0.1f, 2.0f)] private float _fadeInDuration = 0.5f;
        [SerializeField] [Range(0.1f, 4.0f)] private float _stayDuration = 1.0f;
        [SerializeField] [Range(0.1f, 2.0f)] private float _fadeOutDuration = 0.5f;

        private VisualElement _logo;

        void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            _logo = root.Q<VisualElement>("Logo");
            _fadeLogo().Forget();
        }

        private async UniTask _fadeLogo()
        {
            await UniTask.DelayFrame(1);
            await _logo.FadeToAsync(1f, _fadeInDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(_stayDuration));
            await _logo.FadeToAsync(0f, _fadeOutDuration);
        }
    }
}
using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine.UIElements;
using System.Threading;
using Foxbyte.Presentation.Extensions;

namespace FHH.UI.Intro
{
    public class IntroView : ViewBase<IntroPresenter>
    {
        protected override string LocalizationTableName => "GeneralTable";
        protected override bool AutoHideOnClickOutside { get; }
        protected override bool IsModal { get; }

        private CancellationTokenSource _cts;
        private VisualElement _generatedView;

        public override async UniTask InitAsync()
        {
            await base.InitAsync();
            _cts = new CancellationTokenSource();
        }

        public async UniTask FadeOutAsync()
        {
            await _generatedView.Q<VisualElement>("intro-view").FadeToAsync(0f,0.3f);
        }

        protected override async UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer)
        {
            IntroViewGenerator viewGenerator = ViewGeneratorBase<IntroPresenter, IntroView>.Create<IntroViewGenerator>(this);
            _generatedView = await viewGenerator.GenerateViewAsync();
            return _generatedView;
        }
    }
}
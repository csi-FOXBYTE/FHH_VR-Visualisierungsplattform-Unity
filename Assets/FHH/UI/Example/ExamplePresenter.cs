using Cysharp.Threading.Tasks;
using System;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Example
{
    public sealed class ExamplePresenter : PresenterBase<ExamplePresenter, ExampleView, ExampleModel>
    {
        public ExamplePresenter(
            GameObject targetGameobjectForView,
            UIDocument uidoc,
            StyleSheet styleSheet,
            VisualElement target,
            ExampleModel model = null,
            Foxbyte.Core.Services.Permission.RequiredPermissions perms = null)
            : base(targetGameobjectForView, uidoc, styleSheet, target, model ?? new ExampleModel(), perms)
        {
        }

        public override async UniTask InitializeBeforeUiAsync()
        {
            // Model-only init; View not available yet
            await base.InitializeBeforeUiAsync();
        }

        public override async UniTask InitializeAfterUiAsync()
        {
            await UniTask.Yield();

            // View is ready here. Wire events, push initial data, start UI-lifetime jobs, etc.
            var helloButton = View.RootOfThisView.Q<Button>("ExampleHelloButton");
            if (helloButton == null)
            {
                Debug.LogError("ExampleHelloButton not found.");
                return;
            }

            helloButton.clicked += OnHelloButtonClicked;
        }

        private void OnHelloButtonClicked()
        {
            // Minimal “button mash” prevention (provided by PresenterBase)
            var src = View.RootOfThisView.Q<VisualElement>("ExampleHelloButton");
            if (src != null && !Allow(src)) return;

            Debug.Log("Hello Hamburg");
        }

        protected override async UniTask OnViewUnmountedAsync()
        {
            // Unsubscribe from events here (safe cleanup point)
            if (View?.RootOfThisView != null)
            {
                var helloButton = View.RootOfThisView.Q<Button>("ExampleHelloButton");
                if (helloButton != null)
                    helloButton.clicked -= OnHelloButtonClicked;
            }

            await UniTask.CompletedTask;
        }
    }
}
using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Help
{
    public sealed class HelpView : ViewBase<HelpPresenter>
    {
        protected override string LocalizationTableName => "Help"; 
        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;

        protected override async UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer)
        {
            var gen = ViewGeneratorBase<HelpPresenter, HelpView>.Create<HelpViewGenerator>(this);
            return await gen.GenerateViewAsync();
        }
    }
}
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FHH.Logic.Components.Collaboration;
using Foxbyte.Presentation;
using UnityEngine.UIElements;

namespace FHH.UI.Collaboration
{
    public sealed class CollaborationView : ViewBase<CollaborationPresenter>
    {
        private CollaborationViewGenerator _generator;

        protected override string LocalizationTableName => "General";
        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;

        protected override async UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer)
        {
            _generator = ViewGeneratorBase<CollaborationPresenter, CollaborationView>.Create<CollaborationViewGenerator>(this);
            return await _generator.GenerateViewAsync();
        }

        // Called by presenter to refresh the list
        public void RebuildList(List<MeetingEvent> meetings)
        {
            _generator?.BuildCards(meetings);
        }
    }
}
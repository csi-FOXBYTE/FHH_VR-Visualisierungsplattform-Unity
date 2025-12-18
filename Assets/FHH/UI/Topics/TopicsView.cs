using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine.UIElements;

namespace FHH.UI.Topics
{
    public sealed class TopicsView : ViewBase<TopicsPresenter>
    {
        protected override string LocalizationTableName => "General";
        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;

        private ListView _downloadLv;
        private ListView _availableLv;

        protected override async UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer)
        {
            var gen = ViewGeneratorBase<TopicsPresenter, TopicsView>.Create<TopicsViewGenerator>(this);
            return await gen.GenerateViewAsync();
        }

        // Public hooks for presenter
        public void BindDownload(IList<TopicsModel.TopicItem> items,
            //System.Action<TopicsModel.TopicItem> onInfo,
            System.Action<TopicsModel.TopicItem> onDelete,
            System.Action<TopicsModel.TopicItem, bool> onToggle)
        {
            _downloadLv.itemsSource = items as System.Collections.IList
                                        ?? (System.Collections.IList)items; // items are List<TopicItem> in our model
            _downloadLv.userData = new DownloadContext
            {
                //OnInfo = onInfo,
                OnDelete = onDelete,
                OnToggle = onToggle
            };
            _downloadLv.Rebuild();
        }

        public void BindAvailable(IList<TopicsModel.TopicItem> items,
            //System.Action<TopicsModel.TopicItem> onInfo,
            System.Action<TopicsModel.TopicItem, VisualElement> onDownload,
            System.Action<TopicsModel.TopicItem, bool> onToggle)
        {
            _availableLv.itemsSource = items as System.Collections.IList
                                    ?? (System.Collections.IList)items;
            _availableLv.userData = new AvailableContext
            {
                //OnInfo = onInfo,
                OnDownload = onDownload,
                OnToggle = onToggle
            };
            _availableLv.Rebuild();
        }

        // Helpers the generator can set after creation
        internal void SetDownloadList(ListView lv) => _downloadLv = lv;
        internal void SetAvailableList(ListView lv) => _availableLv = lv;

        // Progress helpers
        public ProgressBar ShowProgressFor(VisualElement buttonContainer)
        {
            buttonContainer.Clear();
            var pb = new ProgressBar { name = "ItemProgress" };
            pb.AddToClassList("item-progress");
            pb.lowValue = 0;
            pb.highValue = 1f;
            pb.value = 0f;
            buttonContainer.Add(pb);
            return pb;
        }
        public void UpdateProgress(ProgressBar bar, float normalized) => bar.value = normalized;

        // Smart string values if binding to placeholders later
        //public override Dictionary<string, string> GetSmartStringParameters(VisualElement element) => null;

        public sealed class DownloadContext
        {
            //public System.Action<TopicsModel.TopicItem> OnInfo;
            public System.Action<TopicsModel.TopicItem> OnDelete;
            public System.Action<TopicsModel.TopicItem, bool> OnToggle;
        }

        public sealed class AvailableContext
        {
            //public System.Action<TopicsModel.TopicItem> OnInfo;
            public System.Action<TopicsModel.TopicItem, VisualElement> OnDownload;
            public System.Action<TopicsModel.TopicItem, bool> OnToggle;
        }
        
    }
}
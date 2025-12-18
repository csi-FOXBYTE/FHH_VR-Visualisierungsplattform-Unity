using System;
using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using Foxbyte.Presentation.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Topics
{
    public sealed class TopicsViewGenerator : ViewGeneratorBase<TopicsPresenter, TopicsView>
    {
        private StyleBackground _infoTex;
        private StyleBackground _downloadTex;
        private StyleBackground _deleteTex;

        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            LoadIcons();

            var root = new VisualElement { name = "TopicsRoot" };
            root.AddToClassList("col");
            root.style.width = Length.Percent(100);

            // Tabs bar
            var bar = root.CreateChild("row");
            bar.name = "TopicsBar";
            var btnBasis = new Button { name = "TopicsBasis"};
            btnBasis.AddToClassList("btn");
            btnBasis.AddToClassList("btn-primary");
            btnBasis.AddToClassList("topics-bar-Btn");
            //var btnMore  = new Button { name = "TopicsMore"};
            //btnMore.AddToClassList("btn");
            //btnMore.AddToClassList("topics-bar-Btn");
            bar.Add(btnBasis);
            //bar.Add(btnMore);

            // Download header
            var dHdr = root.CreateChild("section-header");
            dHdr.name = "TopicsDownloadHeader";
            var dTitle = new Label();
            dTitle.name = "TopicsDownloadTitle";
            dTitle.AddToClassList("section-title");
            dHdr.Add(dTitle);

            const float RowHeight = 48f;
            // Download List
            var download = new ListView { name = "TopicsDownload" };
            download.selectionType = SelectionType.None;
            download.fixedItemHeight = RowHeight;
            download.makeItem = MakeDownloadItem;
            download.bindItem = BindDownloadItem;
            root.Add(download);

            // Available header
            var aHdr = root.CreateChild("section-header");
            aHdr.name = "TopicsAvailableHeader";
            var aTitle = new Label();
            aTitle.name = "TopicsAvailableTitle";
            aTitle.AddToClassList("section-title");
            aHdr.Add(aTitle);

            // Available List
            var available = new ListView { name = "TopicsAvailable" };
            available.selectionType = SelectionType.None;
            available.fixedItemHeight = RowHeight;
            available.makeItem = MakeAvailableItem;
            available.bindItem = BindAvailableItem;
            root.Add(available);

            // Give lists to view so presenter can bind
            View.SetDownloadList(download);
            View.SetAvailableList(available);

            await UniTask.Yield();
            return root;
        }

        private void LoadIcons()
        {
            //Texture2D Info()     => Resources.Load<Texture2D>("Icons/info_i");
            Texture2D Download() => Resources.Load<Texture2D>("Icons/download");
            Texture2D Delete()   => Resources.Load<Texture2D>("Icons/delete");

            //_infoTex     = new StyleBackground(Info());
            _downloadTex = new StyleBackground(Download());
            _deleteTex   = new StyleBackground(Delete());
        }

        //  Item Factories 

        private VisualElement MakeDownloadItem()
        {
            var row = new VisualElement();
            row.AddToClassList("topic-item");

            var left = row.CreateChild("topic-left", "row");

            var toggle = new Toggle { name = "ItemToggle" };
            toggle.AddToClassList("i18n-skip");
            left.Add(toggle);

            var title = new Label { name = "ItemTitle" };
            title.AddToClassList("topic-title");
            title.AddToClassList("i18n-skip");
            left.Add(title);

            var right = row.CreateChild("topic-right", "row");

            var size = new Label { name = "ItemSize" };
            size.AddToClassList("topic-size");
            size.AddToClassList("i18n-skip");
            right.Add(size);

            var date = new Label { name = "ItemDate" };
            date.AddToClassList("topic-date");
            date.AddToClassList("i18n-skip");
            right.Add(date);

            //var infoWrap = right.CreateChild("info-wrap");
            //infoWrap.AddToClassList("i18n-skip");
            //var info = new VisualElement { name = "ItemInfo" };
            //info.style.backgroundImage = _infoTex;
            //info.style.width = 16; info.style.height = 16;
            //info.AddToClassList("i18n-skip");
            //infoWrap.Add(info);

            // no i18n-skip (buttons inside must be localized)
            var actionHost = new VisualElement { name = "ItemAction" };
            right.Add(actionHost);

            View.BindLocalizationFor(row);

            return row;
        }
        
        private void BindDownloadItem(VisualElement row, int index)
        {
            var list   = row.GetFirstAncestorOfType<ListView>();
            var items  = (System.Collections.IList)list.itemsSource;
            var item   = (TopicsModel.TopicItem)items[index];

            //row.Q<Toggle>("ItemToggle").value = item.IsSelected;
            //row.Q<Toggle>("ItemToggle").RegisterValueChangedCallback(e => item.IsSelected = e.newValue);

            row.Q<Label>("ItemTitle").text = item.Title;
            row.Q<Label>("ItemSize").text  = TopicsPresenter.FormatSize(item.Bytes);
            row.Q<Label>("ItemDate").text  = item.DownloadedAt.HasValue ? item.DownloadedAt.Value.ToString("dd.MM.yyyy") : "";

            var toggle = row.Q<Toggle>("ItemToggle");

            // Unhook old handler if present (recycled row safety)
            if (toggle.userData is EventCallback<ChangeEvent<bool>> oldCb)
                toggle.UnregisterValueChangedCallback(oldCb);

            toggle.SetValueWithoutNotify(item.IsSelected);
            EventCallback<ChangeEvent<bool>> cb = e =>
            {
                item.IsSelected = e.newValue;
                if (list.userData is TopicsView.DownloadContext ctx)
                    ctx.OnToggle?.Invoke(item, e.newValue);
            };
            toggle.RegisterValueChangedCallback(cb);
            toggle.userData = cb;

            //var infoEl = row.Q<VisualElement>("ItemInfo");
            //if (infoEl != null)
            //{
            //    if (infoEl.userData is EventCallback<PointerDownEvent> oldInfoCb)
            //        infoEl.UnregisterCallback<PointerDownEvent>(oldInfoCb);

            //    EventCallback<PointerDownEvent> infoCb = _ =>
            //    {
            //        if (list.userData is TopicsView.DownloadContext  actx)
            //            actx.OnInfo?.Invoke(item);
            //    };
            //    infoEl.RegisterCallback<PointerDownEvent>(infoCb);
            //    infoEl.userData = infoCb;
            //}
            
            var actionHost = row.Q("ItemAction");
            actionHost.Clear();

            if (item.IsDownloading)
            {
                var pb = new ProgressBar { name = "ItemProgress" };
                pb.AddToClassList("item-progress");
                pb.AddToClassList("i18n-skip");     // progress has no localization
                pb.lowValue = 0; pb.highValue = 1f; pb.value = item.DownloadProgress;
                actionHost.Add(pb);
            }
            else
            {
                var btn = new Button { name = "Topics_ItemDeleteBtn" };
                btn.AddToClassList("icon-btn");  // keep localizable

                var trash = new VisualElement { name = "ItemDeleteIcon" };
                trash.style.backgroundImage = _deleteTex;
                btn.Add(trash);

                // Unhook old if present
                if (btn.userData is EventCallback<ClickEvent> old)
                    btn.UnregisterCallback<ClickEvent>(old);


                EventCallback<ClickEvent> cbd = _ =>
                {
                    if (list.userData is TopicsView.DownloadContext ctx)
                        ctx.OnDelete?.Invoke(item);
                };
                btn.RegisterCallback(cbd);
                btn.userData = cbd;
                actionHost.Add(btn);
            }
            View.BindLocalizationFor(row);
        }


        private VisualElement MakeAvailableItem()
        {
            var row = new VisualElement();
            row.AddToClassList("topic-item");

            var left = row.CreateChild("topic-left", "row");

            var toggle = new Toggle { name = "ItemToggle" };
            toggle.AddToClassList("i18n-skip");
            left.Add(toggle);

            var title = new Label { name = "ItemTitle" };
            title.AddToClassList("topic-title");
            title.AddToClassList("i18n-skip");
            left.Add(title);

            var right = row.CreateChild("topic-right", "row");

            var size = new Label { name = "ItemSize" };
            size.AddToClassList("topic-size");
            size.AddToClassList("i18n-skip");
            right.Add(size);

            //var infoWrap = right.CreateChild("info-wrap");
            //infoWrap.AddToClassList("i18n-skip");
            //var info = new VisualElement { name = "ItemInfo" };
            //info.style.backgroundImage = _infoTex;
            //info.style.width = 16; info.style.height = 16;
            //info.AddToClassList("i18n-skip");
            //infoWrap.Add(info);

            var actionHost = new VisualElement { name = "ItemAction" };
            right.Add(actionHost);

            // no i18n-skip here
            var btn = new Button { name = "Topics_ItemDownloadBtn" };
            btn.AddToClassList("icon-btn");

            var dl = new VisualElement { name = "ItemDownloadIcon" };
            dl.style.backgroundImage = _downloadTex;
            //dl.AddToClassList("i18n-skip");
            btn.Add(dl);

            actionHost.Add(btn);

            View.BindLocalizationFor(row);

            return row;
        }

        private void BindAvailableItem(VisualElement row, int index)
        {
            var list  = row.GetFirstAncestorOfType<ListView>();
            var items = (System.Collections.IList)list.itemsSource;
            var item  = (TopicsModel.TopicItem)items[index];

            //row.Q<Toggle>("ItemToggle").value = item.IsSelected;
            //row.Q<Toggle>("ItemToggle").RegisterValueChangedCallback(e => item.IsSelected = e.newValue);

            row.Q<Label>("ItemTitle").text = item.Title;
            row.Q<Label>("ItemSize").text  = TopicsPresenter.FormatSize(item.Bytes);

            var toggle = row.Q<Toggle>("ItemToggle");

            if (toggle.userData is EventCallback<ChangeEvent<bool>> oldCb)
                toggle.UnregisterValueChangedCallback(oldCb);

            toggle.SetValueWithoutNotify(item.IsSelected);

            EventCallback<ChangeEvent<bool>> cbt = e =>
            {
                item.IsSelected = e.newValue;
                if (list.userData is TopicsView.AvailableContext ctx)
                    ctx.OnToggle?.Invoke(item, e.newValue);
            };
            toggle.RegisterValueChangedCallback(cbt);
            toggle.userData = cbt;

            //var infoEl = row.Q<VisualElement>("ItemInfo");
            //if (infoEl != null)
            //{
            //    if (infoEl.userData is EventCallback<PointerDownEvent> oldInfoCb)
            //        infoEl.UnregisterCallback<PointerDownEvent>(oldInfoCb);

            //    EventCallback<PointerDownEvent> infoCb = _ =>
            //    {
            //        if (list.userData is TopicsView.AvailableContext actx)
            //            actx.OnInfo?.Invoke(item);
            //    };
            //    infoEl.RegisterCallback<PointerDownEvent>(infoCb);
            //    infoEl.userData = infoCb;
            //}

            var actionHost = row.Q("ItemAction");
            var btn = actionHost.Q<Button>("Topics_ItemDownloadBtn");
            if (btn != null)
            {
                // Unhook any previous handler from recycled row
                if (btn.userData is EventCallback<ClickEvent> old)
                    btn.UnregisterCallback(old);

                EventCallback<ClickEvent> cb = _ =>
                {
                    if (list.userData is TopicsView.AvailableContext ctx)
                        ctx.OnDownload?.Invoke(item, actionHost);
                };
                btn.RegisterCallback(cb);
                btn.userData = cb;
            }
        }
    }
}
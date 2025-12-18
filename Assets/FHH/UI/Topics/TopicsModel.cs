using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using FHH.Logic;
using FHH.Logic.Models;
using Foxbyte.Core.Services.Permission;
using UnityEngine;

namespace FHH.UI.Topics
{
    public sealed class TopicsModel : PresenterModelBase
    {
        public enum TopicGroup { Basis, More }

        public sealed class TopicItem
        {
            public string Id;
            public string Title;
            public long Bytes;
            public DateTime? DownloadedAt;

            public bool IsSelected;
            public bool IsDownloading;
            public float DownloadProgress; // 0..1 (UI only)

            public string Href;
            public BaseLayerType Type;
            public bool IsDownloaded;

            public bool IsRestricted;
        }

        public TopicGroup CurrentGroup { get; private set; } = TopicGroup.Basis;
        
        public readonly List<TopicItem> BasisDownloaded = new();
        public readonly List<TopicItem> BasisRemote     = new();
        //public readonly List<TopicItem> MoreDownloaded  = new();
        //public readonly List<TopicItem> MoreRemote      = new();

        public override async UniTask InitializeAsync()
        {
            await LoadFromLayerManagerAsync();
        }

        public async UniTask ReloadFromManagerAsync()
        {
            await LoadFromLayerManagerAsync();
        }

        private async UniTask LoadFromLayerManagerAsync()
        {
            ClearAll();

            var lm = LayerManager.Instance;
            if (lm == null)
            {
                Debug.LogWarning("TopicsModel: LayerManager.Instance is null; showing empty lists.");
                return;
            }
            
            // ensure latest download flags (in case app just started or after a download)
            await lm.RefreshDownloadedStatusAsync();

            var combined = lm.GetCombinedBaseLayers();
            if (combined == null) return;

            foreach (var c in combined)
            {
                var bytes = SizeGbToBytes(c.SizeGb);
                
                var item = new TopicItem
                {
                    Id           = c.Id,
                    Title        = string.IsNullOrWhiteSpace(c.Name) ? c.Id : c.Name,
                    Bytes        = bytes,
                    DownloadedAt = c.DownloadedAt,
                    Href         = c.Href,
                    Type         = c.Type,
                    IsDownloaded = c.IsDownloaded,
                    IsSelected   = c.IsVisible,
                    IsRestricted = c.IsRestricted
                };

                //var isBasis = (c.Type == BaseLayerType.Terrain || c.Type == BaseLayerType.Imagery);

                if (item.IsDownloaded)
                {
                    //if (isBasis) BasisDownloaded.Add(item);
                    //else MoreDownloaded.Add(item);
                    BasisDownloaded.Add(item);
                }
                else
                {
                    //if (isBasis) BasisRemote.Add(item);
                    //else MoreRemote.Add(item);
                    BasisRemote.Add(item);
                }
                //if (item.IsRestricted) item.Title += "  🔒";
            }
            // sort
            BasisDownloaded.Sort((a, b) => string.Compare(a.Title, b.Title, StringComparison.OrdinalIgnoreCase));
            BasisRemote.Sort((a, b) => string.Compare(a.Title, b.Title, StringComparison.OrdinalIgnoreCase));
            //MoreDownloaded.Sort((a, b) => string.Compare(a.Title, b.Title, StringComparison.OrdinalIgnoreCase));
            //MoreRemote.Sort((a, b) => string.Compare(a.Title, b.Title, StringComparison.OrdinalIgnoreCase));
        }

        private static long SizeGbToBytes(float? gb)
        {
            if (!gb.HasValue || gb.Value <= 0f) return 0L;
            const double factor = 1024.0 * 1024.0 * 1024.0;
            return (long)(gb.Value * factor);
        }

        private void ClearAll()
        {
            BasisDownloaded.Clear();
            BasisRemote.Clear();
            //MoreDownloaded.Clear();
            //MoreRemote.Clear();
        }

        public (IList<TopicItem> downloaded, IList<TopicItem> remote) GetActiveLists()
        {
            //return CurrentGroup == TopicGroup.Basis
            //    ? (BasisDownloaded, BasisRemote)
            //    : (MoreDownloaded, MoreRemote);
            return (BasisDownloaded, BasisRemote);
        }

        public void SwitchGroup(TopicGroup group) => CurrentGroup = group;
    }
}
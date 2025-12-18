using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FHH.Logic;
using FHH.Logic.Models;
using Foxbyte.Presentation;

namespace FHH.UI.Variants
{
    public sealed class VariantsModel : PresenterModelBase
    {
        private Project _currentProject;
        private readonly List<ProjectVariant> _variants = new List<ProjectVariant>();

        public Project CurrentProject => _currentProject;
        public IReadOnlyList<ProjectVariant> Variants => _variants;
        public int SelectedVariantIndex { get; set; }

        public override async UniTask InitializeAsync()
        {
            await base.InitializeAsync();
        }

        /// <summary>
        /// Placeholder that loads the current project and extracts its variants.
        /// </summary>
        public async UniTask LoadCurrentProjectAsync()
        {
            if (CancellationToken.IsCancellationRequested)
            {
                return;
            }
            _variants.Clear();
            
            _currentProject = LayerManager.Instance.GetCurrentProject();

            if (_currentProject?.Variants == null || _currentProject.Variants.Count == 0)
            {
                SelectedVariantIndex = -1;
                await UniTask.CompletedTask;
                return;
            }

            _variants.AddRange(_currentProject.Variants);
            
            var activeVariant = LayerManager.Instance.GetCurrentVariant();
            if (activeVariant != null)
            {
                var index = _variants.FindIndex(v => v != null && v.Id == activeVariant.Id);
                SelectedVariantIndex = index >= 0 ? index : 0;
            }
            else if (SelectedVariantIndex < 0 || SelectedVariantIndex >= _variants.Count)
            {
                SelectedVariantIndex = 0;
            }
            await UniTask.CompletedTask; }

        public IReadOnlyList<string> GetVariantNames()
        {
            if (_variants.Count == 0)
            {
                return Array.Empty<string>();
            }
            var names = new string[_variants.Count];
            for (int i = 0; i < _variants.Count; i++)
            {
                names[i] = string.IsNullOrWhiteSpace(_variants[i]?.Name)
                    ? $"Variante {i + 1}"
                    : _variants[i].Name;
            }
            return names;
        }

        public ProjectVariant GetVariantByIndex(int index)
        {
            if (index < 0 || index >= _variants.Count)
            {
                return null;
            }

            SelectedVariantIndex = index;
            return _variants[index];
        }
    }
}

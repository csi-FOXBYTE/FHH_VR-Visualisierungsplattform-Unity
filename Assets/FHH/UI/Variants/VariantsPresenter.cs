using Cysharp.Threading.Tasks;
using Foxbyte.Core.Services.Permission;
using System;
using FHH.Logic;
using FHH.Logic.Components.Collaboration;
using FHH.Logic.Components.Networking;
using FHH.Logic.Models;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Variants
{
    public sealed class VariantsPresenter : PresenterBase<VariantsPresenter, VariantsView, VariantsModel>
    {
        private CollaborationService _collab;
        
        public VariantsPresenter(
            GameObject targetGameObjectForView,
            UIDocument uiDocument,
            StyleSheet styleSheet,
            VisualElement targetContainer,
            VariantsModel model = null,
            RequiredPermissions perms = null)
            : base(targetGameObjectForView, uiDocument, styleSheet, targetContainer, model ?? new VariantsModel(), perms)
        {
        }
        
        public override async UniTask InitializeBeforeUiAsync()
        {
            await base.InitializeBeforeUiAsync();
            _collab = ServiceLocator.GetService<CollaborationService>();
        }

        public override async UniTask InitializeAfterUiAsync()
        {
            await base.InitializeAfterUiAsync();
            LayerManager.Instance.OnProjectChanged += OnProjectChanged;
            await LoadProjectAsync();
        }

        private void OnProjectChanged(Project p)
        {
            LoadProjectAsync().Forget();
        }

        private async UniTask LoadProjectAsync()
        {
            await Model.LoadCurrentProjectAsync();

            if (View == null || View.RootOfThisView == null) return;
            if (Model.Variants == null || Model.Variants.Count == 0) return;
            var radioGroup = View.RootOfThisView.Q<RadioButtonGroup>("VariantsRadioGroup");
            if (radioGroup == null)
            {
                Debug.LogError("VariantsPresenter: RadioButtonGroup 'VariantsRadioGroup' not found.");
                return;
            }

            var names = Model.GetVariantNames();
            View.SetVariants(names, Model.SelectedVariantIndex);
            radioGroup.SetValueWithoutNotify(Model.SelectedVariantIndex);
            
            radioGroup.RegisterValueChangedCallback(evt =>
            {
                UniTask.Void(async () =>
                {
                    if (!Allow(radioGroup))
                    {
                        return;
                    }
                    await OnVariantSelectedAsync(evt.newValue);
                });
            });
        }

        /// <summary>
        /// Called whenever the user selects a new variant from the radio group.
        /// </summary>
        private async UniTask OnVariantSelectedAsync(int selectedIndex)
        {
            Model.SelectedVariantIndex = selectedIndex;
            var variant = Model.GetVariantByIndex(selectedIndex);
            if (variant == null)
            {
                return;
            }
            await LayerManager.Instance.SetVariantAsync(variant);
            //await LayerManager.Instance.SetVariantByIdAsync(variant.Id);
            Debug.Log($"VariantsPresenter: Selected variant index {selectedIndex}, id '{variant.Id}', name '{variant.Name}'.");

            if (!_collab.IsGuidedModeEnabled) return;
            if (!ServiceLocator.GetService<PermissionService>().IsModerator()) return;
            SendCommandAsync("Variant", variant: variant.Id).Forget();
        }

        private async UniTask SendCommandAsync(string type,
            bool value = false,
            double x = 0, double y = 0, double z = 0,
            string projectId = null,
            string variant = null,
            bool enabled = false,
            Quaternion rotation = default,
            int year = 0, int month = 0, int day = 0, int hour = 0, int minute = 0
        )
        {
            var json = JsonUtility.ToJson(new CommandPayload
            {
                Type = type,
                Value = value,
                X = x,
                Y = y,
                Z = z,
                ProjectId = projectId,
                Variant = variant,
                Enabled = enabled,
                Rotation = rotation,
                Year = year,
                Month = month,
                Day = day,
                Hour = hour,
                Minute = minute
            });

            var vivoxHandler = ServiceLocator.GetService<UGSService>().GetModule<VivoxHandler>();
            vivoxHandler.SendCommandMessageAsync(json, _collab.CommandChannelName).Forget();
            await UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            LayerManager.Instance.OnProjectChanged -= OnProjectChanged;
        }
    }
}

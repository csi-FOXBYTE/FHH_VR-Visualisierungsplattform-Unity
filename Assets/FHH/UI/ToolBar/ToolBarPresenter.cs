using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FHH.Input;
using FHH.Logic;
using FHH.Logic.Components.Collaboration;
using FHH.Logic.Components.Laserpointer;
using FHH.Logic.Components.Networking;
using FHH.Logic.Models;
using FHH.UI.Variants;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using FHH.UI.Startingpoints;
using FHH.UI.Sun;
using Foxbyte.Core;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace FHH.UI.ToolBar
{
    public class ToolBarPresenter
        : PresenterBase<ToolBarPresenter, ToolBarView, ToolBarModel>
    {
        public ToolBarPresenter(
            GameObject targetGameObjectForView,
            UIDocument uiDoc,
            StyleSheet styleSheet,
            VisualElement targetContainer,
            ToolBarModel model = null,
            RequiredPermissions perms = null)
            : base(targetGameObjectForView, uiDoc, styleSheet, targetContainer, model ?? new ToolBarModel())
        {
        }

        private PlayerController _pc;
        private CollaborationService _collab; 
        private ICommandBus _commandBus;
        private LaserPointSelector _laserPointSelector;

        public override async UniTask InitializeBeforeUiAsync()
        {
            await base.InitializeBeforeUiAsync();
            _pc = Object.FindFirstObjectByType<PlayerController>();
            if (_pc == null)
            {
                Debug.LogError("PlayerController not found.");
            }

            try
            {
                _collab = ServiceLocator.GetService<CollaborationService>();
                _commandBus = ServiceLocator.GetService<CommandBusService>();
                _commandBus.GuidedModeChanged += SetGuidedMode;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to get CollaborationService or CommandBusService: {ex.Message}");
            }

            LayerManager.Instance.OnProjectUnloaded += HandleProjectUnloaded;
            LayerManager.Instance.OnProjectUnloadingStarted += HandleProjectUnloadingStarted;
        }
        

        public override async UniTask InitializeAfterUiAsync()
        {
            await UniTask.Yield();
            View.ApplyModelToView(Model);
            WireUpCallbacks();
        
            _laserPointSelector = Object.FindFirstObjectByType<LaserPointSelector>();
            if (_laserPointSelector == null)
            {
                Debug.LogError("LaserPointSelector not found in scene.");
            }

            _laserPointSelector.PointSelected += OnPointSelected;
            _laserPointSelector.LaserPointerSelectionFailed += OnPointSelectionFailed;
        }

        private void OnPointSelected(Vector3 position)
        {
            //Debug.Log("Point selected");
            OnLaserPointer(false);
            if (!ServiceLocator.GetService<PermissionService>().IsModerator()) return;
            if (_collab != null)
            {
                if (!_collab.IsGuidedModeEnabled) return;
                var pos = new double3(position);
                SendCommandAsync("Indicator", value:true, x: pos.x, y: pos.y, z: pos.z).Forget();
            }
        }

        private void OnPointSelectionFailed()
        {
            OnLaserPointer(false);
        }

        private void HandleProjectUnloaded()
        {
        }

        /// <summary>
        /// Reset to fly mode when unloading a project so the player
        /// does not fall into the void because the terrain is gone.
        /// </summary>
        private void HandleProjectUnloadingStarted()
        {
            Model.IsWalkMode = false;
            View.UpdateWalkFly(Model.IsWalkMode);
            _pc.SetFlyMode(true);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_commandBus != null)
                _commandBus.GuidedModeChanged -= SetGuidedMode;
            if (LayerManager.Instance != null)
            {
                LayerManager.Instance.OnProjectUnloaded -= HandleProjectUnloaded;
                LayerManager.Instance.OnProjectUnloadingStarted -= HandleProjectUnloadingStarted;
            }
            if (_laserPointSelector != null)
            {
                //_laserPointSelector.PointPlacementCompleted -= OnLayerPointerPlacementCompleted;
                //_laserPointSelector.LaserPointerWaitingChanged -= OnLaserPointerWaitingChanged;
                _laserPointSelector.PointSelected -= OnPointSelected;
            }
        }
        
        private void WireUpCallbacks()
        {
            View.OnStartingPoints = OnStartingPoints;
            View.OnWalkFlyToggled = OnWalkFlyToggled;
            View.OnSpeedModeToggled = OnSpeedModeToggled;
            View.OnLaserPointer = OnLaserPointer;
            View.OnVariants = OnVariants;
            View.OnSunSimulation = OnSunSimulation;
            View.OnToolsToggle = OnToolsToggle;
        }

        public void SetGuidedMode(string projectId, string variant, bool enable)
        {
            Model.IsGuidedMode = enable;
            View.SetGuidedButtonsEnabled(!enable);
        }

        private void OnStartingPoints()
        {
            if (!Allow(View.StartingPointsButton)) return;
            ShowStartingpointsAsync().Forget();
            //var p = LayerManager.Instance.GetCurrentProject();
            //if (p == null) return;
            //if (p.StartingPoints?.Count > 0)
            //{
            //    ShowStartingpointsAsync().Forget();
            //}
        }

        private async UniTask ShowStartingpointsAsync()
        {
            var uiManager = ServiceLocator.GetService<UIManager>();
            if (uiManager.IsShown<StartingpointsPresenter>())
            {
                await uiManager.HideAsync<StartingpointsPresenter>();
                return;
            }
            await uiManager.HideRegionAsync(UIProjectContext.UIRegion.Content1);
            
            var options = new WindowOptions
            {
                StyleSheet = Resources.Load<StyleSheet>("Startingpoints"),
                CenterScreen = false,
                Region = UIProjectContext.UIRegion.Content1
            };

            uiManager.ShowWindowAsync<StartingpointsPresenter, StartingpointsView, StartingpointsModel>(
                model: null,
                options: options,
                initData: new List<StartingPoint>()
                ).Forget();
            
            await UniTask.Yield();
        }

        private void OnWalkFlyToggled(bool isWalkMode)
        {
            if (!Allow(View.WalkFlyButton)) return;

            Model.IsWalkMode = isWalkMode;
            View.UpdateWalkFly(Model.IsWalkMode);
            _pc.SetFlyMode(!isWalkMode);
        }

        private void OnSpeedModeToggled(bool isSprint)
        {
            if (!Allow(View.SpeedModeButton)) return;

            Model.IsSprintMode = isSprint;
            View.UpdateSpeedMode(Model.IsSprintMode);
            // Adjust movement speed based on sprint mode
            _pc.IsDoubleMovementSpeed = isSprint;
        }

        private void OnLaserPointer(bool isOn)
        {
            if (!Allow(View.LaserPointerButton)) return;
            Debug.Log($"OnLaserPointer called with: {isOn}");
            Model.IsLaserOn = isOn;
            View.UpdateLaser(Model.IsLaserOn);
            //View.SetLaserPointerMode(isOn);
            if (!isOn) return;
            //_laserPointSelector.ActivateLaserAndAwaitPoint();
            ActivateLaserPointerAsync().Forget();

            if (!ServiceLocator.GetService<PermissionService>().IsModerator()) return;
            if (_collab != null)
            {
                if (!_collab.IsGuidedModeEnabled) return;
            }
            //SendCommandAsync("Indicator", true, 
            //    x: _pc.transform.position.x, y: _pc.transform.position.y, z: _pc.transform.position.z + 10f).Forget();
        }

        private async UniTask ActivateLaserPointerAsync()
        {
            await UniTask.Yield();
            await UniTask.DelayFrame(10);
            _laserPointSelector.ActivateLaserAndAwaitPoint();
        }

        private void OnVariants()
        {
            if (!Allow(View.VariantsButton)) return;
            var p = LayerManager.Instance.GetCurrentProject();
            if (p == null) return;
            if (p.Variants?.Count > 0)
            {
                ShowVariantsAsync().Forget();
            }
        }

        private async UniTask ShowVariantsAsync()
        {
            var uiManager = ServiceLocator.GetService<UIManager>();
            if (uiManager.IsShown<VariantsPresenter>())
            {
                await uiManager.HideAsync<VariantsPresenter>();
                return;
            }
            await uiManager.HideRegionAsync(UIProjectContext.UIRegion.SidebarContent);
            
            var options = new WindowOptions
            {
                StyleSheet = Resources.Load<StyleSheet>("Variants"),
                CenterScreen = false,
                Region = UIProjectContext.UIRegion.SidebarContent
            };

            uiManager.ShowWindowAsync<VariantsPresenter, VariantsView, VariantsModel>(
                model: null,
                options: options).Forget();


            await UniTask.Yield();
        }

        private void OnSunSimulation()
        {
            if (!Allow(View.SunSimulationButton)) return;
            ShowSunAsync().Forget();
        }

        private async UniTask ShowSunAsync()
        {
            var uiManager = ServiceLocator.GetService<UIManager>();
            if (uiManager.IsShown<SunPresenter>())
            {
                await uiManager.HideAsync<SunPresenter>();
                return;
            }
            await uiManager.HideRegionAsync(UIProjectContext.UIRegion.SidebarContent);
            
            var options = new WindowOptions
            {
                StyleSheet = Resources.Load<StyleSheet>("Sun"),
                CenterScreen = false,
                Region = UIProjectContext.UIRegion.SidebarContent
            };

            uiManager.ShowWindowAsync<SunPresenter, SunView, SunModel>(
                model: null,
                options: options).Forget();

            await UniTask.Yield();
        }

        private void OnToolsToggle(bool isOpen)
        {
            if (!Allow(View.ToolsButton)) return;

            Model.IsOpen = isOpen;
            View.SetToolbarOpenClosedAsync(isOpen).Forget();
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
            try
            {
                var vivoxHandler = ServiceLocator.GetService<UGSService>().GetModule<VivoxHandler>();
                vivoxHandler.SendCommandMessageAsync(json, _collab.CommandChannelName).Forget();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to send command message: {ex.Message}");
            }

            await UniTask.CompletedTask;
        }
    }
}
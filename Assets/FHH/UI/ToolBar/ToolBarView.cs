using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using Foxbyte.Presentation.Extensions;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using VisualElementExtensions = Foxbyte.Presentation.Extensions.VisualElementExtensions;

namespace FHH.UI.ToolBar
{
    public class ToolBarView : ViewBase<ToolBarPresenter>
    {
        public Button StartingPointsButton { get; private set; }
        public Button WalkFlyButton { get; private set; }
        public Button SpeedModeButton { get; private set; } 
        public Button LaserPointerButton { get; private set; }
        public Button VariantsButton { get; private set; }
        public Button SunSimulationButton { get; private set; }
        public Button ToolsButton { get; private set; } // bottom persistent toggle

        private VisualElement _stack; // container for the 6 top buttons

        // Callbacks provided by presenter
        public Action OnStartingPoints;
        public Action<bool> OnWalkFlyToggled;
        public Action<bool> OnSpeedModeToggled;
        public Action<bool> OnLaserPointer;
        public Action OnVariants;
        public Action OnSunSimulation;
        public Action<bool> OnToolsToggle;

        protected override string LocalizationTableName => "General";
        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;

        protected override async UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument,
            VisualElement targetContainer)
        {
            var gen = ViewGeneratorBase<ToolBarPresenter, ToolBarView>.Create<ToolBarViewGenerator>(this);
            var root = await gen.GenerateViewAsync();

            _stack = root.Q<VisualElement>("TB_Stack");
            StartingPointsButton = root.Q<Button>("TB_StartingPoints");
            WalkFlyButton = root.Q<Button>("TB_WalkFly");
            SpeedModeButton = root.Q<Button>("TB_SpeedMode");
            LaserPointerButton = root.Q<Button>("TB_LaserPointer");
            VariantsButton = root.Q<Button>("TB_Variants");
            SunSimulationButton = root.Q<Button>("TB_SunSimulation");
            ToolsButton = root.Q<Button>("TB_Tools");

            WireHandlers();

            return root;
        }

        public void ApplyModelToView(ToolBarModel model)
        {
            UpdateWalkFly(model.IsWalkMode);
            UpdateSpeedMode(model.IsSprintMode);
            UpdateLaser(model.IsLaserOn);
            //UpdateVariants();
            //UpdateSunSimulation();
            SetToolbarOpenClosedAsync(model.IsOpen).Forget();
            SetGuidedButtonsEnabled(!model.IsGuidedMode);
        }

        public void SetGuidedButtonsEnabled(bool enable)
        {
            StartingPointsButton.SetEnabled(enable);
            LaserPointerButton.SetEnabled(enable);
            VariantsButton.SetEnabled(enable);
            SunSimulationButton.SetEnabled(enable);
        }

        private void WireHandlers()
        {
            StartingPointsButton.clicked += () => OnStartingPoints?.Invoke();

            WalkFlyButton.clicked += () =>
            {
                bool newWalk = !WalkFlyButton.ClassListContains("tb-btn--toggled");
                OnWalkFlyToggled?.Invoke(newWalk);
            };

            SpeedModeButton.clicked += () =>
            {
                bool newSprint = !SpeedModeButton.ClassListContains("tb-btn--toggled");
                OnSpeedModeToggled?.Invoke(newSprint);
            };

            LaserPointerButton.clicked += () =>
            {
                bool newOn = !LaserPointerButton.ClassListContains("tb-btn--toggled");
                OnLaserPointer?.Invoke(newOn);
            };

            VariantsButton.clicked += () =>
            {
               OnVariants?.Invoke();
            };

            SunSimulationButton.clicked += () =>
            {
                OnSunSimulation?.Invoke();
            };

            ToolsButton.clicked += () =>
            {
                bool newOpen = !ToolsButton.ClassListContains("tb-tools--open");
                OnToolsToggle?.Invoke(newOpen);
            };
        }

        
        public void UpdateWalkFly(bool isWalkMode)
        {
            SetToggled(WalkFlyButton, isWalkMode);
            SetIcon(WalkFlyButton, isWalkMode ? "pedestrian_view" : "bird_view");
        }

        public void UpdateSpeedMode(bool isSprint)
        {
            SetToggled(SpeedModeButton, isSprint);
            SetIcon(SpeedModeButton, isSprint ? "sprint" : "walk");
        }

        public void UpdateLaser(bool isOn)
        {
            SetToggled(LaserPointerButton, isOn);
            SetIcon(LaserPointerButton, "laserpointer");
        }

        //public void UpdateVariants(bool isOn)
        //{
        //    SetIcon(VariantsButton, "variants");
        //}

        //public void UpdateSunSimulation(bool isOn)
        //{
        //    SetIcon(SunSimulationButton, "sun_simulation");
        //}

        public async UniTaskVoid SetToolbarOpenClosedAsync(bool isOpen)
        {
            ToolsButton.EnableInClassList("tb-tools--open", isOpen);
            ToolsButton.EnableInClassList("tb-tools--closed", !isOpen);

            if (isOpen)
            {
                for (var i = _stack.childCount - 1; i >= 0; i--)
                {
                    var child = _stack.ElementAt(i);
                    child.pickingMode = PickingMode.Position;
                    child.FadeToAsync(0.95f, 0.1f).Forget();
                    await UniTask.WaitForSeconds(0.03f);
                }
            }
            else
            {
                for (var i = 0; i < _stack.childCount; i++)
                {
                    var child = _stack.ElementAt(i);
                    child.pickingMode = PickingMode.Ignore;
                    child.FadeToAsync(0.0f, 0.1f).Forget();
                    await UniTask.WaitForSeconds(0.03f);
                }
            }
        }

        private void SetToggled(VisualElement ve, bool on)
        {
            ve.EnableInClassList("tb-btn--toggled", on);
            ve.EnableInClassList("tb-btn--untoggled", !on);
        }

        private void SetIcon(VisualElement ve, string iconNameNoExt)
        {
            var icon = ve.Q<VisualElement>("Icon");
            if (icon == null) return;

            var sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>($"Icons/ToolBar/{iconNameNoExt}");
            icon.style.backgroundImage = sprite != null ? new StyleBackground(sprite) : new StyleBackground();
        }

        // Laser indicator management

        
        
    }
}
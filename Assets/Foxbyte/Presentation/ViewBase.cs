using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UIElements;

namespace Foxbyte.Presentation
{
    public abstract class ViewBase : MonoBehaviour
    {
        public UIDocument UIDocOfThisView;
        public StyleSheet StyleSheetOfThisView;
        public VisualElement RootOfThisView;
        public VisualElement ContainerOfThisView;

        private CancellationTokenSource _mountCts;
        public CancellationToken MountToken { get; private set; }


        private void RefreshMountCts()
        {
            _mountCts?.Cancel();
            _mountCts?.Dispose();
            _mountCts = new CancellationTokenSource();
            MountToken = _mountCts.Token;
        }

        // Call this when the view is (re)initialized
        protected void BeginMountLifetime()
        {
            RefreshMountCts();
        }
        
        protected virtual void OnDestroy()
        {
            _mountCts?.Cancel();
            _mountCts?.Dispose();
            _mountCts = null;
        }
    }
    
    public abstract class ViewBase<TPresenter> : ViewBase where TPresenter : IPresenter
    {
        public TPresenter Presenter;
        /// <summary>
        /// The name of the localization table that this view uses.
        /// Example: protected override string LocalizationTableName => "GeneralTable";
        /// </summary>
        protected abstract string LocalizationTableName { get; }
        
        protected abstract bool AutoHideOnClickOutside { get; }
        protected abstract bool IsModal { get; } //blocks click events outside this UI

        // to skip elements that should not be localized
        private const string I18nSkip = "i18n-skip";
        private const string I18nSkipSubtree = "i18n-skip-subtree";

        /// <summary>
        /// Override this to provide dynamic values for smart strings.
        /// On localization update, it will call this method to get the latest values.
        /// Return a dictionary where keys correspond to placeholders in smart strings.
        /// Example: return new Dictionary&lt;string,string> { { "someKey", "someValue" } };
        /// Note: For smart strings, create the entry in localization table and set it as smart string.
        /// Because every smart string can be individual, you need to override GetSmartStringParameters()
        /// method to provide the dynamic values and have a specific method for each smart string you want
        /// to update at runtime, where you can call SetSmartStringParameter() for each key-value pair.
        /// </summary>
        public virtual Dictionary<string, string> GetSmartStringParameters(VisualElement element)
        {
            return null;
            //examples:
            //if (element.name == "MeasurementLabel")
            //{
            //    return new Dictionary<string, string>
            //    {
            //        ["value"] = Presenter.GetMeasurementValue(),
            //        ["unit"]  = Presenter.GetMeasurementUnit() //cm, inch, ...
            //    };
            //}
            //or:
            //if (element.name == "ProgressLabel")
            //    return new() {
            //        { "currentStep", Presenter.GetStep() },
            //        { "totalSteps" , Presenter.GetTotalSteps() }
            //    };
            // with ProgressLabel entry in table: "Step {currentStep} of {totalSteps}"
            //or completely different: register a global variable once like this:
            //var globals = LocalizationSettings.StringDatabase
            //    .SmartFormatter
            //    .GetSourceExtension<PersistentVariablesSource>();
            //globals["global"]["playerName"] = new StringVariable { Value = "Fritz" };
            // any entry in table can do: "Hello {global.playerName}!"
        }

        /// <summary>
        /// Update a single smart-string placeholder on an element at runtime.
        /// Finds the IVariable in userData and sets its Value, triggering an auto-refresh.
        /// Example:
        /// MeasurementLabel = "{value} {unit}" in localization table (set as smart string).
        /// public void UpdateMeasurement(float value, string unit)
        /// {
        /// var label = RootOfThisView.Q<Label>("MeasurementLabel");
        /// SetSmartStringParameter(label, "value", value.ToString());
        /// SetSmartStringParameter(label, "unit",  unit);
        /// </summary>
        public void SetSmartStringParameter(VisualElement element, string key, string value)
        {
            if (element.userData is Dictionary<string, IVariable> vars 
                && vars.TryGetValue(key, out var iv) 
                && iv is StringVariable sv)
            {
                sv.Value = value;
                //Debug.Log($"Set smart-string '{element.name}' key '{key}' to '{value}'");
            }
            else
            {
                Debug.LogError($"Smart-string key '{key}' not found on element '{element.name}'.");
            }
        }

        /// <summary>
        /// Call this from Presenter to refresh all smart string values in the view.
        /// Example (based on Measurement example above),in Presenter:
        /// public void OnUserChangedValue(float newValue, string newUnit)
        /// {
        /// Model.Value = newValue;
        /// Model.Unit  = newUnit;
        /// View.RefreshAllSmartStringValues();
        ///}
        /// </summary>
        public void RefreshAllSmartStringValues()
        {
            void Recurse(VisualElement ve)
            {
                if (ve.userData is Dictionary<string, IVariable>)
                {
                    var newVars = GetSmartStringParameters(ve);
                    if (newVars != null)
                    {
                        foreach (var (k, v) in newVars)
                        {
                            SetSmartStringParameter(ve, k, v);
                            //Debug.Log($"Refreshed smart-string '{ve.name}' with {k}: {v}");
                        }
                    }
                }
                foreach (var child in ve.Children()) Recurse(child);
            }
            Recurse(RootOfThisView);
        }

        /// <summary>
        /// Will be called from presenter automatically.
        /// Override this method as a replacement for Unity's Start() method if you need more complex initialization logic.
        /// Reminder: You need to call "await base.InitAsync();" first.
        /// </summary>
        /// <returns></returns>
        public virtual async UniTask InitAsync()
        {
            BeginMountLifetime();
            await UniTask.Yield();
            if (StyleSheetOfThisView && !UIDocOfThisView.rootVisualElement.styleSheets.Contains(StyleSheetOfThisView))
            {
                UIDocOfThisView.rootVisualElement.styleSheets.Add(StyleSheetOfThisView);
            }

            //if (RootOfThisView != null && RootOfThisView.parent != null)
            //    RootOfThisView.RemoveFromHierarchy();
            
            var generatedView = await SetUpVisualElement(UIDocOfThisView, ContainerOfThisView);

            //if (generatedView.parent != null)
            //    generatedView.RemoveFromHierarchy();
            
            RootOfThisView = generatedView;

            AutoHide(generatedView);
            ModalSetup(generatedView);
            ContainerOfThisView.Add(generatedView);

            BindLocalization();
            RefreshAllSmartStringValues();
            
            //Presenter?.MarkUnmounted(); // ensure stale flags cleared if any stray state 

            await UniTask.CompletedTask;
        }

        protected virtual void OnDisable()
        {
            ClearUserDataRecursive(RootOfThisView);
        }

        /// <summary>
        /// Automatically hides the view when clicking outside of the target container.
        /// Checks if AutoHideOnClickOutside is true, and if so, registers a PointerDownEvent callback.
        /// </summary>
        /// <param name="targetContainer"></param>
        private void AutoHide(VisualElement targetContainer)
        {
            if (AutoHideOnClickOutside)
            {
                targetContainer.RegisterCallback<PointerDownEvent>(evt =>
                {
                    //if (!targetContainer.Contains(evt.target as VisualElement)) // which is better?
                    if (!targetContainer.worldBound.Contains(evt.position))
                    {
                        evt.StopPropagation();   
                        //UIManager.HideView...
                        Debug.LogError($"Needs proper UIManager call: Hiding view {targetContainer.name} because clicked outside of it.");
                        Destroy(this);
                        //targetContainer.visible = false; 
                        //targetContainer.style.display = DisplayStyle.None;
                        //targetContainer.RemoveFromHierarchy();
                    }
                }, TrickleDown.TrickleDown);
            }
        }


        private void ModalSetup(VisualElement targetContainer)
        {
            if (!IsModal) return;
            var blocker = new VisualElement() { name = "ModalBlocker" };
            blocker.style.position = Position.Absolute;
            blocker.pickingMode = PickingMode.Position;
            blocker.style.top = blocker.style.left = 0;
            blocker.style.right = blocker.style.bottom = 0;
            blocker.style.backgroundColor = new Color(0, 0, 0, 0);
            blocker.RegisterCallback<PointerDownEvent>(evt => evt.StopPropagation(), TrickleDown.TrickleDown);
            blocker.RegisterCallback<WheelEvent>(evt => evt.StopPropagation(), TrickleDown.TrickleDown);
            
            blocker.SendToBack();
            Debug.LogError("Sending Modal blocker to back. Is this working?"); //maybe add after Add if not working

            targetContainer.Add(blocker);
        }

        /// <summary>
        /// In here, set up the view generator for this view.
        /// Example:
        /// IntroViewGenerator viewGenerator = ViewGeneratorBase<IntroPresenter, IntroView>.Create<IntroViewGenerator>(this);
        /// return await viewGenerator.GenerateViewAsync();
        /// </summary>
        protected abstract UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer);

        /// <summary>
        /// Not needed, but can be overridden to handle locale changes.
        /// Subscribe to this event in InitAsync() to get notified when the locale changes:
        /// LocalizationSettings.SelectedLocaleChanged += SelectedLocaleChanged;
        /// </summary>
        protected virtual void SelectedLocaleChanged(Locale locale)
        {
        }

        /// <summary>
        /// Called once when the view is initialized to bind localization to the UI elements.
        /// </summary>
        protected void BindLocalization()
        {
            if (string.IsNullOrEmpty(LocalizationTableName))
                throw new InvalidOperationException($"{nameof(LocalizationTableName)} not set in {GetType().Name}");

            // walk tree one time
            BuildBindingsRecursive(RootOfThisView);
        }

        public void BindLocalizationFor(VisualElement subtree)
        {
            if (string.IsNullOrEmpty(LocalizationTableName))
                throw new InvalidOperationException($"{nameof(LocalizationTableName)} not set in {GetType().Name}");
            if (subtree == null) return;

            BuildBindingsRecursive(subtree); // reuse the existing walker
        }

        private void BuildBindingsRecursive(VisualElement ve)
        {
            // Skip whole subtree?
            if (ve.ClassListContains(I18nSkipSubtree)) return;

            // Skip this element’s own binding?
            bool skipThis = ve.ClassListContains(I18nSkip);


            if (!skipThis && !string.IsNullOrEmpty(ve.name))
            {
                var ls = new LocalizedString(LocalizationTableName, ve.name);
                var vars = GetSmartStringParameters(ve);
                if (vars != null && vars.Count > 0)
                {
                    // attach a map of IVariable instances to the element
                    var map = new Dictionary<string, IVariable>();
                    foreach (var (k, v) in vars)
                    {
                        var sv = new StringVariable { Value = v };
                        ls[k] = sv;
                        map[k] = sv;
                    }
                    ve.userData = map;
                }
                ve.SetBinding("text", ls);
            }

            foreach (var child in ve.Children())
                BuildBindingsRecursive(child);
        }

        private void ClearUserDataRecursive(VisualElement ve)
        {
            ve.userData = null;
            foreach (var child in ve.Children())
                ClearUserDataRecursive(child);
        }

        // Optional helper to mark at runtime
        public void MarkNoLocalization(VisualElement ve, bool includeChildren = false)
        {
            ve.AddToClassList(includeChildren ? I18nSkipSubtree : I18nSkip);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (RootOfThisView != null && RootOfThisView.parent != null)
                RootOfThisView.RemoveFromHierarchy();
            if (StyleSheetOfThisView && UIDocOfThisView && UIDocOfThisView.rootVisualElement != null)
                UIDocOfThisView.rootVisualElement.styleSheets.Remove(StyleSheetOfThisView);

            RootOfThisView = null;
            ContainerOfThisView = null;
            Presenter?.MarkUnmounted();
        }
    }
}
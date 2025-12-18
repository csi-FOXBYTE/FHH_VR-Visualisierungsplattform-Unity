using System;
using Cysharp.Threading.Tasks;
using FHH.Logic.Models;
using FHH.UI;
using FHH.UI.Attribute;
using Foxbyte.Presentation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace FHH.Logic.Cesium
{
    public class CustomAttributeXRHandler : MonoBehaviour
    {
        //private XRInteractionManager _interactionManager;
        private InteractionLayerMask _buildingSelectLayerMask;
        private XRSimpleInteractable _interactable;
        private bool _initialized;

        void Start()
        {
            //_interactionManager = FindFirstObjectByType<XRInteractionManager>();
            //if (_interactionManager == null)
            //{
            //    Debug.LogError("Assign a XRInteractionManager.");
            //}
            _buildingSelectLayerMask = InteractionLayerMask.GetMask("BuildingSelect");
            if (_buildingSelectLayerMask == 0)
            {
                Debug.LogError("Assign a buildingSelect layer mask.");
            }
        }

       void OnEnable()
        {
            _initialized = false;
            TryInitialize();
        }

        void Update()
        {
            if (!_initialized)
            {
                TryInitialize();
            }
        }

        private void OnDisable()
        {
            if (_interactable != null)
            {
                _interactable.activated.RemoveListener(OnActivated);
            }

            _initialized = false;
        }

        private void TryInitialize()
        {
            if (_initialized)
            {
                return;
            }

            if (!HasCollider())
            {
                return;
            }

            if (!TryGetComponent(out _interactable))
            {
                _interactable = gameObject.AddComponent<XRSimpleInteractable>();
            }

            _interactable.interactionLayers = _buildingSelectLayerMask;
            _interactable.activated.AddListener(OnActivated);

            _initialized = true;
        }

        private bool HasCollider()
        {
            if (TryGetComponent<Collider>(out _))
            {
                return true;
            }

            return GetComponentInChildren<Collider>() != null;
        }

        private void OnActivated(ActivateEventArgs args)
        {
            OnInteract();
        }

        private void OnInteract()
        {
            Debug.Log($"Selected building {gameObject.name}");

            var projectModelAttributes = gameObject.GetComponentInParent<ProjectModelAttributes>();
            if (projectModelAttributes != null)
            {
                var sourceAttributes = projectModelAttributes.Attributes;
                var buildingMetadata = sourceAttributes != null
                    ? new Dictionary<string, string>(sourceAttributes)
                    : new Dictionary<string, string>();
                ShowAttributeAsync(buildingMetadata).Forget();
            }
            else
            {
                Debug.LogError("No project model attributes found on selected building.");
            }
        }

        private async UniTask ShowAttributeAsync(Dictionary<string, string> metaData)
        {
            var uiManager = ServiceLocator.GetService<UIManager>();
            if (uiManager.IsShown<AttributePresenter>())
            {
                await uiManager.HideAsync<AttributePresenter>();
                return;
            }

            await uiManager.HideRegionAsync(UIProjectContext.UIRegion.Content1);

            var options = new WindowOptions
            {
                StyleSheet = Resources.Load<StyleSheet>("Attribute"),
                CenterScreen = false,
                Region = UIProjectContext.UIRegion.Content1
            };

            uiManager.ShowWindowAsync<AttributePresenter, AttributeView, AttributeModel>(
                model: null,
                initData: metaData,
                options: options).Forget();

            await UniTask.Yield();
        }
    }
}
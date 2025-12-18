using Cysharp.Threading.Tasks;
using FHH.Input;
using FHH.Logic;
using FHH.Logic.Models;
using Foxbyte.Core;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Startingpoints
{
    public class StartingpointsPresenter 
        : PresenterBase<StartingpointsPresenter, StartingpointsView, StartingpointsModel>
    {
        public StartingpointsPresenter(
            GameObject targetGameobjectForView,
            UIDocument uidoc,
            StyleSheet styleSheet,
            VisualElement target,
            StartingpointsModel model = null,
            RequiredPermissions perms = null)
            : base(targetGameobjectForView, uidoc, styleSheet, target, model ?? new StartingpointsModel(), perms)
        {
        }

        // called once the presenter is created
        public override async UniTask InitializeBeforeUiAsync()
        {
            await base.InitializeBeforeUiAsync();
        }

        // called every time a new view is about to be created
        public override async UniTask InitializeWithDataBeforeUiAsync<T>(T data)
        {
            await base.InitializeWithDataBeforeUiAsync(data);
            await Model.InitializeWithDataAsync(data);
            // load public starting points
            await Model.GetPublicStartingPointsAsync();
            // try to load current project and overwrite public starting points if found
            var project = await LoadCurrentProjectAsync();
            if (project != null)
            {
                Model.SetProject(project);
            }
        }

        public override async UniTask InitializeAfterUiAsync()
        {
            await base.InitializeAfterUiAsync();
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// Loads the current project from the LayerManager.
        /// </summary>
        private async UniTask<Project> LoadCurrentProjectAsync()
        {
            await UniTask.Yield();
            return LayerManager.Instance.GetCurrentProject();
        }

        /// <summary>
        /// Called by the view/generator when a starting point card is clicked.
        /// </summary>
        /// <param name="startingPoint"></param>
        public void OnStartingPointCardClicked(StartingPoint startingPoint)
        {
            if (startingPoint == null)
            {
                return;
            }
            HandleStartingPointSelection(startingPoint.Origin, startingPoint.Target);
        }

        /// <summary>
        /// Move rig to the selected starting point.
        /// Orient camera to the target.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        private void HandleStartingPointSelection(Vector3d origin, Vector3d target)
        {
            var geoRefGo = GameObject.FindGameObjectWithTag("CesiumGeoRef");
            var geoRef = geoRefGo != null ? geoRefGo.GetComponent<CesiumForUnity.CesiumGeoreference>() : null;
            if (geoRef == null)
            {
                ULog.Warning("CesiumGeoreference not found on CesiumGeoRef. Skipping polygon creation.");
                return;
            }
            var originD3 = new double3(origin.X, origin.Y, origin.Z);
            var originVector3 = geoRef.TransformEarthCenteredEarthFixedPositionToUnity(originD3);
            var targetVector3 = geoRef.TransformEarthCenteredEarthFixedPositionToUnity(new double3(target.X, target.Y, target.Z));
            // create rotation quaternion looking from origin to target
            var direction = (targetVector3 - originVector3);
            var rot = Quaternion.LookRotation(new Vector3((float)direction.x, (float)direction.y, (float)direction.z));

            var pctrl = GameObject.FindFirstObjectByType<PlayerController>();
            if (pctrl == null)
            {
                ULog.Error("PlayerController not found in scene. Cannot move rig to starting point.");
                return;
            }
            pctrl.TeleportTo(originD3, rot, true);
            //ULog.Info($"Teleporting to position: {originVector3} with rotation: {rot}");
        }
    }
}

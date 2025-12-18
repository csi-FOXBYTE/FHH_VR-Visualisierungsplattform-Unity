using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace Foxbyte.Presentation.Rendering
{
    public class UIAfterEverythingFeature : ScriptableRendererFeature
    {
        public enum Strategy
        {
            ClearDepthAfterScene, // preferred for UI Toolkit
            DrawUiLayerOnTop // optional: Mesh/Sprite renderers
        }

        [System.Serializable]
        public class Settings
        {
            public Strategy Mode = Strategy.ClearDepthAfterScene;
            public LayerMask UiLayer = 0; // used only by DrawUiLayerOnTop
            public Material OverrideMaterial; // optional (DrawUiLayerOnTop)
            public RenderPassEvent InjectionPoint = RenderPassEvent.BeforeRenderingTransparents;
            public bool BaseCamerasOnly = true; // skip overlay cameras
            public bool SkipXRMultipass = false; // set true to avoid double work in multipass XR
            public bool TransparentQueueOnly = true; // trim renderer list to transparents
            public bool NoSortingIfFewObjects = true;
        }

        public Settings Config = new Settings();

        private ClearDepthPass _clearDepthPass;
        private DrawUiLayerPass _drawUiLayerPass;

        public override void Create()
        {
            _clearDepthPass = new ClearDepthPass(Config.InjectionPoint);
            _drawUiLayerPass = new DrawUiLayerPass(Config.InjectionPoint, Config.UiLayer, Config.OverrideMaterial);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var camData = renderingData.cameraData;

            if (camData.cameraType != CameraType.Game) return;
            if (Config.BaseCamerasOnly && camData.renderType != CameraRenderType.Base) return;
            if (Config.SkipXRMultipass && camData.xr.enabled && !camData.xr.singlePassEnabled) return;

            if (Config.Mode == Strategy.ClearDepthAfterScene)
            {
                _clearDepthPass.SetInjectionPoint(Config.InjectionPoint);
                renderer.EnqueuePass(_clearDepthPass);
            }
            else
            {
                _drawUiLayerPass.Rebind(Config.UiLayer, Config.OverrideMaterial,
                    Config.TransparentQueueOnly, Config.NoSortingIfFewObjects);
                _drawUiLayerPass.SetInjectionPoint(Config.InjectionPoint);
                renderer.EnqueuePass(_drawUiLayerPass);
            }
        }

        private class ClearDepthPass : ScriptableRenderPass
        {
            private const string _tag = "UI After Everything: Clear Depth";
            private readonly ProfilingSampler _sampler = new ProfilingSampler(_tag);

            public ClearDepthPass(RenderPassEvent evt)
            {
                renderPassEvent = evt;
            }

            public void SetInjectionPoint(RenderPassEvent evt)
            {
                renderPassEvent = evt;
            }

            private class PassData
            {
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                using (var builder = renderGraph.AddRasterRenderPass<PassData>(_tag, out var passData, _sampler))
                {
                    var resources = frameData.Get<UniversalResourceData>();
                    builder.SetRenderAttachmentDepth(resources.activeDepthTexture, AccessFlags.Write);

                    builder.SetRenderFunc((PassData _, RasterGraphContext ctx) =>
                    {
                        // Clear only depth; keep color intact. XR-safe.
                        ctx.cmd.ClearRenderTarget(clearDepth: true, clearColor: false, backgroundColor: Color.black);
                    });
                }
            }
        }

        // draw a layer "on top" (for Mesh/Sprite renderers)
        private class DrawUiLayerPass : ScriptableRenderPass
        {
            private const string _tag = "UI After Everything: Draw UI Layer On Top";
            private readonly ProfilingSampler _sampler = new ProfilingSampler(_tag);

            private bool _transparentOnly;
            private bool _noSortingIfFew;


            private static readonly List<ShaderTagId> _shaderTags = new List<ShaderTagId>
            {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("SRPDefaultUnlit")
            };

            private LayerMask _layerMask;
            private Material _overrideMaterial;

            public DrawUiLayerPass(RenderPassEvent evt, LayerMask layerMask, Material overrideMat)
            {
                renderPassEvent = evt;
                _layerMask = layerMask;
                _overrideMaterial = overrideMat;
            }

            public void SetInjectionPoint(RenderPassEvent evt)
            {
                renderPassEvent = evt;
            }

            public void Rebind(LayerMask layerMask, Material overrideMat)
            {
                _layerMask = layerMask;
                _overrideMaterial = overrideMat;
            }

            public void Rebind(LayerMask layerMask, Material overrideMat, bool transparentOnly, bool noSortingIfFew)
            {
                _layerMask = layerMask;
                _overrideMaterial = overrideMat;
                _transparentOnly = transparentOnly;
                _noSortingIfFew = noSortingIfFew;
            }

            private class PassData
            {
                public RendererListHandle RendererList;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var urp = frameData.Get<UniversalRenderingData>();
                var cam = frameData.Get<UniversalCameraData>().camera;

                var rq = _transparentOnly ? RenderQueueRange.transparent : RenderQueueRange.all;
                // iIf set is tiny and we don't care about strict back-to-front, skip sorting to cut CPU
                var criteria = _noSortingIfFew ? SortingCriteria.None : SortingCriteria.CommonTransparent;

                var state = new RenderStateBlock(RenderStateMask.Depth)
                {
                    depthState = new DepthState(writeEnabled: false, compareFunction: CompareFunction.Always) // on-top
                };

                var desc = new UnityEngine.Rendering.RendererUtils.RendererListDesc(_shaderTags.ToArray(),
                    urp.cullResults, cam)
                {
                    renderQueueRange = rq,
                    sortingCriteria = criteria,
                    layerMask = _layerMask.value,
                    overrideMaterial = _overrideMaterial,
                    overrideMaterialPassIndex = 0,
                    stateBlock = state
                };

                using (var builder = renderGraph.AddRasterRenderPass<PassData>(_tag, out var passData, _sampler))
                {
                    passData.RendererList = renderGraph.CreateRendererList(desc);
                    builder.UseRendererList(passData.RendererList);

                    var res = frameData.Get<UniversalResourceData>();
                    builder.SetRenderAttachment(res.activeColorTexture, 0);
                    builder.SetRenderAttachmentDepth(res.activeDepthTexture, AccessFlags.ReadWrite);

                    builder.AllowPassCulling(true);

                    builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
                    {
                        ctx.cmd.DrawRendererList(data.RendererList);
                    });
                }
            }
        }
    }
}
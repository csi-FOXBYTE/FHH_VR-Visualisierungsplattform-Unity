using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Foxbyte.Presentation.Rendering.BlurFeature
{
    public class BlurRenderPass : ScriptableRenderPass
    {
        private static readonly int _horizontalBlurId = Shader.PropertyToID("_HorizontalBlur");
        private static readonly int _verticalBlurId = Shader.PropertyToID("_VerticalBlur");
        private static readonly int _samplesId = Shader.PropertyToID("_Samples");
        private const string KBlurTextureName = "_BlurTexture";
        private const string KVerticalPassName = "VerticalBlurRenderPass";
        private const string KHorizontalPassName = "HorizontalBlurRenderPass";

        private readonly BlurSettings _defaultSettings;
        private readonly Material _material;

        private TextureDesc _blurTextureDescriptor;

        public BlurRenderPass(Material material, BlurSettings defaultSettings)
        {
            _material = material;
            _defaultSettings = defaultSettings;
        }

        private void UpdateBlurSettings()
        {
            if (_material == null) return;

            var volumeComponent = VolumeManager.instance.stack.GetComponent<CustomVolumeComponent>();
            float horizontalBlur = volumeComponent.HorizontalBlur.overrideState
                ? volumeComponent.HorizontalBlur.value
                : _defaultSettings.horizontalBlur;
            float verticalBlur = volumeComponent.VerticalBlur.overrideState
                ? volumeComponent.VerticalBlur.value
                : _defaultSettings.verticalBlur;
            int samples = volumeComponent.Samples.overrideState
                ? volumeComponent.Samples.value
                : _defaultSettings.samples;
            _material.SetFloat(_horizontalBlurId, horizontalBlur);
            _material.SetFloat(_verticalBlurId, verticalBlur);
            _material.SetInt(_samplesId, samples);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph,
            ContextContainer frameData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            if (resourceData.isActiveTargetBackBuffer)
                return;

            TextureHandle srcCamColor = resourceData.activeColorTexture;
            _blurTextureDescriptor = srcCamColor.GetDescriptor(renderGraph);
            _blurTextureDescriptor.name = KBlurTextureName;
            _blurTextureDescriptor.depthBufferBits = 0;
            var dst = renderGraph.CreateTexture(_blurTextureDescriptor);

            UpdateBlurSettings();

            if (!srcCamColor.IsValid() || !dst.IsValid())
                return;

            RenderGraphUtils.BlitMaterialParameters paraVertical = new(srcCamColor, dst, _material, 0);
            renderGraph.AddBlitPass(paraVertical, KVerticalPassName);

            RenderGraphUtils.BlitMaterialParameters paraHorizontal = new(dst, srcCamColor, _material, 1);
            renderGraph.AddBlitPass(paraHorizontal, KHorizontalPassName);
        }
    }
}
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Foxbyte.Presentation.Rendering.FadeOverlayFeatureSRP
{
    [DisallowMultipleComponent]
    public class FadeOverlayFeatureSRP : ScriptableRendererFeature
    {
        public Color FadeColor = Color.gray;
        public Shader Shader;
        [Range(0f, 1f)] public float Intensity = 1f;
        public RenderPassEvent InjectionPoint = RenderPassEvent.BeforeRenderingPostProcessing;
        public bool PreviewInEditor;

        private Material _Material;
        private ColorBlitPass _RenderPass;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (!PreviewInEditor && !Application.isPlaying) return; // no cost in Edit Mode
#endif
            const float Epsilon = 1e-4f;
            if (Intensity <= Epsilon) return; // no pass when fully transparent
            
            if (_Material == null)
                return;

            if (renderingData.cameraData.cameraType != CameraType.Game)
                return;

            _RenderPass.SetIntensity(Intensity);
            _RenderPass.SetColor(FadeColor);
            _RenderPass.renderPassEvent = InjectionPoint;
            renderer.EnqueuePass(_RenderPass);
        }

        public override void Create()
        {
            _Material = CoreUtils.CreateEngineMaterial(Shader);
            _RenderPass = new ColorBlitPass(_Material);
            _RenderPass.renderPassEvent = InjectionPoint;
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(_Material);
        }

        public class ColorBlitPass : ScriptableRenderPass
        {
            private const string KPassName = "ColorBlitPass";
            private Material _material;
            private float _intensity;
            private Color _color;
            private static readonly int _intensityID = Shader.PropertyToID("_Intensity");
            private static readonly int _colorID = Shader.PropertyToID("_Color");

            public ColorBlitPass(Material mat)
            {
                _material = mat;
            }

            public void SetIntensity(float intensity)
            {
                _intensity = intensity;
            }

            public void SetColor(Color color)
            {
                _color = color;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resourceData = frameData.Get<UniversalResourceData>();

                // The following line ensures that the render pass doesn't blit
                // from the back buffer.
                if (resourceData.isActiveTargetBackBuffer)
                {
                    Debug.LogError(
                        $"Skipping render pass. ColorBlitRendererFeature requires an intermediate ColorTexture, we can't use the BackBuffer as a texture input.");
                    return;
                }

                var source = resourceData.activeColorTexture;

                // Define the texture descriptor for creating the destination render graph texture.
                var destinationDesc = renderGraph.GetTextureDesc(source);
                destinationDesc.name = $"CameraColor-{KPassName}";
                destinationDesc.clearBuffer = false;
                destinationDesc.depthBufferBits = 0;

                // Create the texture.
                TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

                // The AddBlitPass method adds the render graph pass that blits from the source to the destination texture.
                RenderGraphUtils.BlitMaterialParameters para = new(source, destination, _material, 0);
                para.material.SetFloat(_intensityID, _intensity);
                para.material.SetColor(_colorID, _color);
                renderGraph.AddBlitPass(para, passName: KPassName);

                // Use the destination texture as the camera texture to avoid the extra blit from the destination texture back to the camera texture.
                resourceData.cameraColor = destination;
            }
        }
    }
}
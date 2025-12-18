using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Foxbyte.Presentation.Rendering.FadeOverlayFeature
{
    public class FadeOverlayFeature : ScriptableRendererFeature
    {
        public enum CameraFilter
        {
            AllCameras,
            BaseOnly,
            OverlayOnly,
            TaggedOnly
        }

        class FullscreenFadePass : ScriptableRenderPass
        {
            private FadeOverlayFeature _owner;

            private class PassData
            {
                public Material OverlayMaterial;
                public MaterialPropertyBlock MPB;
                public Color OverlayColor;
            }

            private static void ExecutePass(PassData data, RasterGraphContext context)
            {
                if (data.OverlayMaterial == null) return;

                data.MPB ??= new MaterialPropertyBlock();
                data.MPB.SetColor("_OverlayColor", data.OverlayColor);
                CoreUtils.DrawFullScreen(context.cmd, data.OverlayMaterial, data.MPB, 0);
            }

            public FullscreenFadePass(FadeOverlayFeature owner)
            {
                _owner = owner;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                const string passName = "Fullscreen Fade Overlay";
                using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
                {
                    var resourceData = frameData.Get<UniversalResourceData>();

                    builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
                    builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Write);

                    var c = _owner._color;
                    c.a *= Mathf.Clamp01(_owner._fade);

                    passData.OverlayColor = c;
                    passData.OverlayMaterial = _owner._overlayMaterial;

                    builder.SetRenderFunc<PassData>(ExecutePass);
                }
            }
        }

        private FullscreenFadePass _scriptablePass;

        [Header("Editor Visibility (Scene View)")] [SerializeField]
        private bool _enabledInEditMode = true;

        [Header("Settings")] [SerializeField] private Color _color = Color.black;
        [SerializeField, Range(0f, 1f)] private float _fade = 1f;
        [SerializeField] private RenderPassEvent _injectionPoint = RenderPassEvent.BeforeRenderingTransparents;

        [Header("Camera Filtering")] [SerializeField]
        private CameraFilter _cameraFilter = CameraFilter.AllCameras;

        [SerializeField] private string _targetCameraTag = "MainCamera";

        [Header("Overlay Material (auto if left empty)")] [SerializeField]
        private Material _overlayMaterial;

        internal static FadeOverlayFeature Instance { get; private set; }

        public Color Color => _color;
        public float Fade => _fade;
        public RenderPassEvent InjectionPoint => _injectionPoint;
        public CameraFilter Filter => _cameraFilter;
        public string TargetCameraTag => _targetCameraTag;
        public bool EnabledInEditMode => _enabledInEditMode;

        public void SetColor(Color color)
        {
            _color = color;
        }

        public void SetFade(float value)
        {
            _fade = Mathf.Clamp01(value);
        }

        public void SetInjectionPoint(RenderPassEvent evt)
        {
            _injectionPoint = evt;
        }

        public void SetEnabledInEditMode(bool enabled)
        {
            _enabledInEditMode = enabled;
        }

        public void SetCameraFilter(CameraFilter filter)
        {
            _cameraFilter = filter;
        }

        public void SetTargetCameraTag(string tag)
        {
            _targetCameraTag = tag;
        }

        public override void Create()
        {
            Instance = this;
            EnsureMaterial();
            _scriptablePass = new FullscreenFadePass(this);
            _scriptablePass.renderPassEvent = _injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!Application.isPlaying && !_enabledInEditMode) return;

            var cam = renderingData.cameraData.camera;

            switch (_cameraFilter)
            {
                case CameraFilter.TaggedOnly:
                    if (string.IsNullOrEmpty(_targetCameraTag) || !cam.CompareTag(_targetCameraTag)) return;
                    break;
                case CameraFilter.BaseOnly:
                case CameraFilter.OverlayOnly:
                    if (cam.TryGetComponent<UniversalAdditionalCameraData>(out var uacd))
                    {
                        if (_cameraFilter == CameraFilter.BaseOnly && uacd.renderType != CameraRenderType.Base) return;
                        if (_cameraFilter == CameraFilter.OverlayOnly &&
                            uacd.renderType != CameraRenderType.Overlay) return;
                    }

                    break;
            }

            _scriptablePass.renderPassEvent = _injectionPoint;
            EnsureMaterial();
            renderer.EnqueuePass(_scriptablePass);
        }

        private void EnsureMaterial()
        {
            if (_overlayMaterial == null)
            {
                var shader = Shader.Find("Hidden/URP/TintOverlay");
                if (shader != null) _overlayMaterial = CoreUtils.CreateEngineMaterial(shader);
            }
        }
    }
}
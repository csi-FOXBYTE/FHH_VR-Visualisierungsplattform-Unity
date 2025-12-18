using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Foxbyte.Presentation.Rendering.BlurFeature
{
    public class BlurRendererFeature : ScriptableRendererFeature
    {
        [FormerlySerializedAs("settings")] [SerializeField]
        private BlurSettings _settings;

        [FormerlySerializedAs("shader")] [SerializeField]
        private Shader _shader;

        private Material _material;
        private BlurRenderPass _blurRenderPass;

        public override void Create()
        {
            if (_shader == null)
            {
                return;
            }

            _material = new Material(_shader);
            _blurRenderPass = new BlurRenderPass(_material, _settings);

            _blurRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer,
            ref RenderingData renderingData)
        {
            if (_blurRenderPass == null)
            {
                return;
            }

            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                renderer.EnqueuePass(_blurRenderPass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (Application.isPlaying)
            {
                Destroy(_material);
            }
            else
            {
                DestroyImmediate(_material);
            }
        }
    }

    [Serializable]
    public class BlurSettings
    {
        [Range(0, 0.4f)] public float horizontalBlur;
        [Range(0, 0.4f)] public float verticalBlur;
        [Range(1, 512)] public int samples = 64;
    }
}
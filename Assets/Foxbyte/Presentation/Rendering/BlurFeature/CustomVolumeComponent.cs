using System;
using UnityEngine.Rendering;

namespace Foxbyte.Presentation.Rendering.BlurFeature
{
    [Serializable]
    public class CustomVolumeComponent : VolumeComponent
    {
        public ClampedFloatParameter HorizontalBlur = new ClampedFloatParameter(0.05f, 0, 0.5f);
        public ClampedFloatParameter VerticalBlur = new ClampedFloatParameter(0.05f, 0, 0.5f);
        public ClampedIntParameter Samples = new ClampedIntParameter(64, 2, 512);
    }
}
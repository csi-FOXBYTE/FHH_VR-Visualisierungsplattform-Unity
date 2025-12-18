namespace FHH.Logic.Models
{
    public class UserSettings
    {
        public string Language { get; set; } = "German";
        public bool PerformanceMode { get; set; } = true;
        public float MoveSpeed { get; set; } = 0.5f;
        public float TurnSpeed { get; set; } = 0.5f;
        public float TurnAngle { get; set; } = 15f;
        public bool VignetteEnabled { get; set; }
        public bool SupportPlatform { get; set; }
        public bool HideTutorial { get; set; }
        public bool HideDisclaimer { get; set; }
    }
}
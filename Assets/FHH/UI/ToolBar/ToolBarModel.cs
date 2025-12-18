using Foxbyte.Presentation;
using Cysharp.Threading.Tasks;

namespace FHH.UI.ToolBar
{
    public class ToolBarModel : PresenterModelBase
    {
        public bool IsOpen { get; set; } = true; // toolbar items visible by default
        public bool IsWalkMode { get; set; } = false; // Walk vs Fly
        public bool IsSprintMode { get; set; } = false; // SpeedMode: Walk vs Sprint
        public bool IsLaserOn { get; set; } = false;
        //public bool IsVariantsOn { get; set; } = false;
        //public bool IsSunSimulationOn { get; set; } = false;
        public bool IsGuidedMode { get; set; } = false;

        public override async UniTask InitializeAsync()
        {
            await UniTask.CompletedTask;
        }
    }
}
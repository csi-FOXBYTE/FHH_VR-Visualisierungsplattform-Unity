using Foxbyte.Presentation;

namespace FHH.UI.MainMenu
{
    public enum MainMenuSection
    {
        Library,
        Meetings,
        Topics,
        Settings,
        Help
    }

    public sealed class MainMenuModel : PresenterModelBase
    {
        public MainMenuSection InitialSection { get; set; } = MainMenuSection.Library;
    }
}
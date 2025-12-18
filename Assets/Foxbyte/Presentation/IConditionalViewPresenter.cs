namespace Foxbyte.Presentation
{
    public interface IConditionalViewPresenter
    {
        /// <summary>
        /// If false, UIManager must not build/show the view at all.
        /// </summary>
        bool ShouldBuildView { get; }
    }
}
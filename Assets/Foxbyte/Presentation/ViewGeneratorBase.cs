using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace Foxbyte.Presentation
{
    /// <summary>
    /// Base class for view generators that create UI elements for a specific view.
    /// Give a name for every view element that needs localization. The same name is the key in the localization table.
    /// Best practice for styling: use a separate uss file for a few specific changes, but use a global uss for most styles.
    /// Try to avoid inline styling.
    /// example use:  public TestGenerator(TestView view, TestPresenter presenter) : base(view, presenter)
    /// </summary>
    public abstract class ViewGeneratorBase<TPresenter,TView> 
        where TPresenter : class, IPresenter
        where TView : ViewBase<TPresenter>
    {
        protected TPresenter Presenter;
        protected TView View;

        protected ViewGeneratorBase() { }

        public abstract UniTask<VisualElement> GenerateViewAsync();

        /// <summary>
        /// Configures the view generator with the provided view. 
        /// </summary>
        /// <param name="view"></param>
        private void Configure(TView view)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));
            Presenter = view.Presenter ?? throw new InvalidOperationException($"{typeof(TView).Name} has no Presenter");
        }

        /// <summary>
        /// Factory method to create a view generator for a specific view type.
        /// </summary>
        /// <typeparam name="TGen"></typeparam>
        /// <param name="view"></param>
        /// <returns></returns>
        public static TGen Create<TGen>(TView view) where TGen : ViewGeneratorBase<TPresenter,TView>, new()
        {
            var gen = new TGen();
            gen.Configure(view);
            return gen;
        }
    }
}
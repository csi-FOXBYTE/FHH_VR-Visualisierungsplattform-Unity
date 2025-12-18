using System.Threading;
using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine.UIElements;

namespace FHH.UI.LocaleList
{
    //public class LocaleListView : ViewBase<LocaleListPresenter>
    //{
    //    private CancellationTokenSource _cts;

    //    protected override string LocalizationTableName => "GeneralTable";

    //protected override async UniTask InitAsync()
    //    {
    //        await base.InitAsync();
    //        _cts = new CancellationTokenSource();
    //    }

    //    public override async UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer)
    //    {
    //        //generate the view
    //        LocaleListViewGenerator viewGenerator = ViewGeneratorBase<LocaleListPresenter, LocaleListView>.Create<LocaleListViewGenerator>(this);
    //        VisualElement generatedView = await viewGenerator.GenerateViewAsync();
    //        return await UniTask.FromResult(generatedView);
    //    }
    //}
}
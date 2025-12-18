using Cysharp.Threading.Tasks;
using FHH.UI.Intro;
using Foxbyte.Core.Localization.Utilities;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.LocaleList
{
   /* public class LocaleListViewGenerator : ViewGeneratorBase<LocaleListPresenter, LocaleListView>
    {
        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            var elem = new VisualElement();
            //elem.name = "LocaleListSubContainer";
            //elem.pickingMode = PickingMode.Ignore;
            //elem.style.flexDirection = FlexDirection.Column;
            //elem.style.justifyContent = Justify.Center;
            //elem.style.alignItems = Align.FlexStart;
            //elem.style.width =Length.Percent(100);
            ////elem.style.height = Length.Percent(100);
            ////elem.style.backgroundColor = new StyleColor(Color.white);
            //elem.style.backgroundColor = Color.clear;
            //elem.style.paddingRight = 50;
            
            ////string[] languages = { "Deutsch", "English", "Español", "Français", "Italiano", "Polski", "Português" };
            //var localSwitcher = ServiceLocator.Instance.GetService<LocaleSwitcher>();
            //var languages = await localSwitcher.GetAvailableLocaleNames();
            //foreach (string language in languages)
            //{
            //    var localeElem = new VisualElement();
            //    localeElem.pickingMode = PickingMode.Ignore;
            //    localeElem.style.flexDirection = FlexDirection.Row;
            //    localeElem.style.justifyContent = Justify.FlexStart;
            //    localeElem.style.alignItems = Align.FlexStart;
            //    localeElem.style.width = Length.Percent(100);
            //    //localeElem.style.marginLeft = Length.Percent(5);
            //    //localeElem.style.marginRight = Length.Percent(5);
            //    //localeElem.style.height = Length.Percent(100);
            //    localeElem.style.height = 48;
            //    localeElem.style.backgroundColor = new StyleColor(Color.white);

            //    Button languageButton = new Button { text = language };
            //    //languageButton.AddToClassList("language-button");
            //    languageButton.style.height = Length.Percent(100);
            //    languageButton.style.height = 40;
            //    languageButton.style.backgroundColor = new StyleColor(Color.white);
            //    languageButton.style.borderBottomColor = Color.clear;
            //    languageButton.style.borderTopColor = Color.clear;
            //    languageButton.style.borderLeftColor = Color.clear;
            //    languageButton.style.borderRightColor = Color.clear;

            //    localeElem.Add(languageButton);

            //    // Add the checkmark if this language is currently active
            //    //if (language == "English")
            //    if (localSwitcher.IsCurrentLocale(language))
            //    {
            //        VisualElement checkmark = new VisualElement();
            //        //checkmark.AddToClassList("checkmark-icon");
            //        var checkmarkPath = "Exclamation";
            //        var checkmarkTex = Resources.Load<Texture2D>(checkmarkPath);

            //        if (checkmarkTex == null)
            //        {
            //            Debug.LogError("Checkmark Image not found");
            //        }

            //        var checkmarkIcon = new Image();
            //        checkmarkIcon.image = checkmarkTex;
            //        checkmarkIcon.style.width = Length.Percent(40);
            //        checkmarkIcon.style.height = Length.Percent(40);
            //        checkmarkIcon.style.alignSelf = Align.Center;
            //        localeElem.Add(checkmarkIcon);
            //    }

            //    languageButton.RegisterCallback<ClickEvent>(evt =>
            //    {
            //        ((LocaleListPresenter)Presenter).LocaleClickHandler(((Button)evt.target).text).Forget();
            //        UIManager.Instance.HideLocaleList().Forget();
            //    });

            //    elem.Add(localeElem);
            //}
 
            return await UniTask.FromResult(elem);
        }
    }*/
}
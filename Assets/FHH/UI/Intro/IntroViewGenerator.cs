using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using Foxbyte.Presentation.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Intro
{
    public class IntroViewGenerator : ViewGeneratorBase<IntroPresenter, IntroView>
    {
        private readonly Color _bgColor = new Color(0f, 51 / 255f, 80 / 255f, 1f);

        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            var elem = new VisualElement();
            elem.name = "intro-view";
            elem.style.flexDirection = FlexDirection.Column;
            elem.style.justifyContent = Justify.Center;
            elem.style.alignItems = Align.Center;
            elem.style.backgroundColor = _bgColor;
            //fill the whole screen
            elem.style.width = Length.Percent(1000);
            elem.style.height = Length.Percent(1000);
            elem.style.position = Position.Absolute;

            var imagePath = "introImage";
            var texture = Resources.Load<Texture2D>(imagePath);

            var logoPath = "logo";
            var textureLogo = Resources.Load<Texture2D>(logoPath);

            if (texture == null)
            {
                Debug.LogError("Image not found at path: " + imagePath);
            }

            var logo = new Image();
            logo.image = textureLogo;
            logo.style.width = 512 /6f;
            logo.style.height = 216 /6f;
            logo.style.marginBottom = 100;
            logo.scaleMode = ScaleMode.ScaleToFit;
            logo.style.opacity = 0;

            var image = new Image();
            image.image = texture;
            image.style.width = 512 /2f;
            image.style.height = 344 /2f;
            image.scaleMode = ScaleMode.ScaleToFit;
            image.style.opacity = 0;
            
            //var title = new Label("Welcome");
            var title = new Label("");
            title.name = "Intro_Title";
            title.style.fontSize = 48;
            title.style.unityTextAlign = TextAnchor.MiddleCenter;
            title.style.color = Color.white;
            title.style.opacity = 0;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            
            var subTitle = new Label("");
            subTitle.name = "Intro_SubTitle";
            subTitle.style.fontSize = 20;
            subTitle.style.unityTextAlign = TextAnchor.MiddleCenter;
            subTitle.style.color = Color.white;
            subTitle.style.opacity = 0;
            subTitle.style.unityFontStyleAndWeight = FontStyle.Normal;
            subTitle.style.marginBottom = 20;
            

            elem.Add(logo);
            elem.Add(title);
            elem.Add(subTitle); 
            elem.Add(image);

            await UniTask.NextFrame();

            title.FadeToAsync(1f, 1f).Forget();
            subTitle.FadeToAsync(1f, 1f).Forget();
            logo.FadeToAsync(1f, 1f).Forget();
            image.FadeToAsync(1f, 1f).Forget();

            return await UniTask.FromResult(elem);
        }
    }
}
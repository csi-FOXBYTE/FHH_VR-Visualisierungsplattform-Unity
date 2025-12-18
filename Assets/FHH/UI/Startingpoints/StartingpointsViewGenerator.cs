using System;
using Cysharp.Threading.Tasks;
using FHH.Logic.Models;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Startingpoints
{
    public class StartingpointsViewGenerator 
        : ViewGeneratorBase<StartingpointsPresenter, StartingpointsView>
    {
        private const string I18nSkipClass = "i18n-skip";
        private const string I18nSkipSubtreeClass = "i18n-skip-subtree";
        private Texture2D _fallbackBackgroundTexture;

        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            await UniTask.Yield();

            var root = new VisualElement
            {
                name = "StartingpointsRoot"
            };
            root.AddToClassList("startingpoints-root");
            root.AddToClassList(I18nSkipClass); 

            var panel = new VisualElement
            {
                name = "StartingpointsPanel"
            };
            panel.AddToClassList("startingpoints-panel");
            panel.AddToClassList("app-background3"); 
            root.Add(panel);

            // Header row with localizable title label
            var headerRow = new VisualElement
            {
                name = "StartingpointsHeader"
            };
            headerRow.AddToClassList("startingpoints-header");
            panel.Add(headerRow);

            var titleLabel = new Label("Startpunkte")
            {
                name = "StartingpointsTitle"
            };
            titleLabel.AddToClassList("startingpoints-title");
            headerRow.Add(titleLabel);

            var contentRoot = new VisualElement
            {
                name = "StartingpointsContent"
            };
            contentRoot.AddToClassList("startingpoints-content");
            contentRoot.AddToClassList(I18nSkipClass);
            contentRoot.AddToClassList(I18nSkipSubtreeClass);
            panel.Add(contentRoot);

            // ScrollView holding the grid
            var scrollView = new ScrollView(ScrollViewMode.Vertical)
            {
                name = "StartingpointsScroll"
            };
            scrollView.AddToClassList("startingpoints-scroll");
            scrollView.AddToClassList(I18nSkipClass);
            contentRoot.Add(scrollView);

            // Grid container (flex-wrap based grid)
            var grid = new VisualElement
            {
                name = "StartingpointsGrid"
            };
            grid.AddToClassList("startingpoints-grid");
            grid.AddToClassList(I18nSkipClass);
            scrollView.contentContainer.Add(grid);

            // Populate cards from model
            if (Presenter?.Model?.StartingPoints != null)
            {
                foreach (var sp in Presenter.Model.StartingPoints)
                {
                    var card = CreateStartingPointCard(sp);
                    grid.Add(card);
                }
            }

            return root;
        }

        private VisualElement CreateStartingPointCard(StartingPoint startingPoint)
        {
            var card = new VisualElement
            {
                name = $"StartingPointCard_{startingPoint.Id}"
            };
            card.AddToClassList("startingpoint-card");
            card.AddToClassList(I18nSkipClass);

            // Background image from base64 texture
            var background = CreateBackgroundFromBase64(startingPoint.Img);
            if (background != null)
            {
                card.style.backgroundImage = new StyleBackground(background);
            }

            // Bottom overlay with text
            var infoOverlay = new VisualElement
            {
                name = $"StartingPointInfo_{startingPoint.Id}"
            };
            infoOverlay.AddToClassList("startingpoint-card-info");
            infoOverlay.AddToClassList(I18nSkipClass);
            card.Add(infoOverlay);

            var nameLabel = new Label(startingPoint.Name ?? string.Empty)
            {
                name = $"StartingPointName_{startingPoint.Id}"
            };
            nameLabel.AddToClassList("startingpoint-card-name");
            nameLabel.AddToClassList(I18nSkipClass);
            infoOverlay.Add(nameLabel);

            var descLabel = new Label(startingPoint.Description ?? string.Empty)
            {
                name = $"StartingPointDesc_{startingPoint.Id}"
            };
            descLabel.AddToClassList("startingpoint-card-description");
            descLabel.AddToClassList(I18nSkipClass);
            infoOverlay.Add(descLabel);

            // Click handling – delegate to presenter
            card.RegisterCallback<ClickEvent>(_ =>
            {
                Presenter?.OnStartingPointCardClicked(startingPoint);
            });

            return card;
        }

        private Texture2D GetFallbackBackground()
        {
            if (_fallbackBackgroundTexture != null)
            {
                return _fallbackBackgroundTexture;
            }

            _fallbackBackgroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            _fallbackBackgroundTexture.wrapMode = TextureWrapMode.Clamp;
            _fallbackBackgroundTexture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.1f)); // black, 10% opacity
            _fallbackBackgroundTexture.Apply();

            return _fallbackBackgroundTexture;
        }

        private Texture2D CreateBackgroundFromBase64(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
            {
                return GetFallbackBackground();
            }

            try
            {
                // Allow optional "data:image/xxx;base64," prefix
                var commaIndex = base64.IndexOf(',');
                if (commaIndex >= 0)
                {
                    base64 = base64.Substring(commaIndex + 1);
                }

                var data = Convert.FromBase64String(base64);
                var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (!ImageConversion.LoadImage(tex, data))
                {
                    UnityEngine.Object.Destroy(tex);
                    return GetFallbackBackground();
                }

                tex.wrapMode = TextureWrapMode.Clamp;
                return tex;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to decode starting point image: {ex.Message}");
                return GetFallbackBackground();
            }
        }
    }
}
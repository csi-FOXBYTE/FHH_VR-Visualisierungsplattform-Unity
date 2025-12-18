using Cysharp.Threading.Tasks;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using System.Collections.Generic;
using FHH.Logic;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Attribute
{
    public sealed class AttributePresenter
        : PresenterBase<AttributePresenter, AttributeView, AttributeModel>
    {
       
        public AttributePresenter(
            GameObject targetGameObjectForView,
            UIDocument uiDocument,
            StyleSheet styleSheet,
            VisualElement targetContainer,
            AttributeModel model = null,
            RequiredPermissions perms = null)
            : base(targetGameObjectForView, uiDocument, styleSheet, targetContainer,
                model ?? new AttributeModel(), perms)
        {
        }

        public IReadOnlyDictionary<string, string> GetAttributes()
        {
            return Model.Attributes;
        }

        public override async UniTask InitializeWithDataBeforeUiAsync<T>(T data)
        {
            await base.InitializeWithDataBeforeUiAsync(data);

            if (data is IDictionary<string, string> dict)
            {
                Model.SetAttributes(new Dictionary<string, string>(dict));
            }
        }

        internal async UniTask HideAsync()
        {
            await ServiceLocator.GetService<UIManager>().HideAsync<AttributePresenter>();
        }
    }
}
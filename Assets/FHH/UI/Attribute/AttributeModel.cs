using System.Collections.Generic;
using Foxbyte.Presentation;

namespace FHH.UI.Attribute
{
    public sealed class AttributeModel : PresenterModelBase
    {
        private readonly Dictionary<string, string> _attributes = new();

        public IReadOnlyDictionary<string, string> Attributes => _attributes;

        public void SetAttributes(IDictionary<string, string> data)
        {
            _attributes.Clear();
            if (data == null)
            {
                return;
            }

            foreach (var kvp in data)
            {
                _attributes[kvp.Key] = kvp.Value;
            }
        }
    }
}
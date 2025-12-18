using System.Collections.Generic;
using UnityEngine;

namespace FHH.Logic.Models
{
    public class ProjectModelAttributes : MonoBehaviour
    {
        public string Id { get; private set; }
        public Dictionary<string, string> Attributes { get; private set; }

        private void Awake()
        {
            if (Attributes == null)
            {
                Attributes = new Dictionary<string, string>();
            }
        }

        public void Initialize(string id, Dictionary<string, string> attributes)
        {
            Id = id;
            if (attributes != null)
            {
                Attributes = new Dictionary<string, string>(attributes);
            }
            else
            {
                Attributes = new Dictionary<string, string>();
            }
        }
    }
}
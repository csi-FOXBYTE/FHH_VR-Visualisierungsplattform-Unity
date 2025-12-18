using System.Collections.Generic;
using CesiumForUnity;
using UnityEngine;

namespace FHH.Logic.Cesium
{
    [RequireComponent(typeof(Cesium3DTileset))]
    public class CesiumDistanceMipBiasController : MonoBehaviour
    {
        [SerializeField] private float _nearDistance = 0.0f;

        [SerializeField] private float _farDistance = 100.0f;

        [SerializeField] private float _nearBias = 0.0f; // close to camera -> keep detail

        [SerializeField] private float _farBias = 1.5f; // far away -> force blurrier mip

        [SerializeField] private int _anisotropicLevel = 4;

        private Cesium3DTileset _tileset;
        private readonly List<TrackedRenderer> _trackedRenderers = new List<TrackedRenderer>();

        private class TrackedRenderer
        {
            public Renderer Renderer;
            public List<Texture2D> Textures = new List<Texture2D>();
        }

        private void Awake()
        {
            _tileset = GetComponent<Cesium3DTileset>();
            _tileset.OnTileGameObjectCreated += OnTileGameObjectCreated;
        }

        private void OnDestroy()
        {
            if (_tileset != null)
            {
                _tileset.OnTileGameObjectCreated -= OnTileGameObjectCreated;
            }
        }

        private void OnTileGameObjectCreated(GameObject tileRoot)
        {
            var renderers = tileRoot.GetComponentsInChildren<Renderer>(true);

            foreach (var renderer in renderers)
            {
                if (renderer == null)
                {
                    continue;
                }

                var tracked = new TrackedRenderer
                {
                    Renderer = renderer
                };

                var materials = renderer.sharedMaterials;
                var addedTextures = new HashSet<Texture2D>();

                foreach (var material in materials)
                {
                    if (material == null)
                    {
                        continue;
                    }

                    var propertyNames = material.GetTexturePropertyNames();
                    foreach (var propertyName in propertyNames)
                    {
                        var tex = material.GetTexture(propertyName) as Texture2D;
                        if (tex == null)
                        {
                            continue;
                        }

                        if (addedTextures.Add(tex))
                        {
                            tex.anisoLevel = _anisotropicLevel;
                            tracked.Textures.Add(tex);
                        }
                    }
                }

                if (tracked.Textures.Count > 0)
                {
                    _trackedRenderers.Add(tracked);
                }
            }
        }

        private void Update()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                return;
            }

            foreach (var tracked in _trackedRenderers)
            {
                if (tracked.Renderer == null)
                {
                    continue;
                }

                var center = tracked.Renderer.bounds.center;
                float distance = Vector3.Distance(cam.transform.position, center);

                float t = 0.0f;
                if (_farDistance > _nearDistance)
                {
                    t = Mathf.InverseLerp(_nearDistance, _farDistance, distance);
                }

                float bias = Mathf.Lerp(_nearBias, _farBias, t);

                foreach (var tex in tracked.Textures)
                {
                    if (tex != null)
                    {
                        tex.mipMapBias = bias;
                    }
                }
            }
        }
    }
}
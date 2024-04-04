using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLight.DmxFixture.Channels
{
    [Serializable]
    public class SyncMaterial
    {
        [SerializeField] private string _materialPropertyName = "";

        [SerializeField] private float _intensityCoefficient = 1f;
        [SerializeField] private float _maxIntensity = 3;
        [SerializeField] private bool _decreasesToBlack = true;

        [SerializeField] private List<MeshRenderer> _meshRenderers = new();

        private Dictionary<MeshRenderer, MaterialPropertyBlock> _materialPropertyBlocks = new();

        public void Init()
        {
            _materialPropertyBlocks?.Clear();
            _materialPropertyBlocks = new Dictionary<MeshRenderer, MaterialPropertyBlock>();

            foreach (var meshRenderer in _meshRenderers)
            {
                if (meshRenderer == null) continue;

                var materialPropertyBlock = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(materialPropertyBlock);
                _materialPropertyBlocks.Add(meshRenderer, materialPropertyBlock);
            }
        }

        public void UpdateMaterials(Color32 color, float intensity)
        {
            if (_materialPropertyBlocks == null)
            {
                Debug.LogError("Required initialization of MaterialPropertyBlocks.");
                Init();
                return;
            }

            intensity = Mathf.Min(intensity * _intensityCoefficient, _maxIntensity);
            var hdrColor = (Color)color * Mathf.Pow(2.0f, intensity);

            var result = _decreasesToBlack
                ? Color.Lerp(Color.black, hdrColor, Mathf.Clamp(intensity, 0, 1f))
                : hdrColor;

            foreach (var (meshRenderer, materialPropertyBlock) in _materialPropertyBlocks)
            {
                materialPropertyBlock.SetColor(_materialPropertyName, result);
                meshRenderer.SetPropertyBlock(materialPropertyBlock);
            }
        }
    }
}

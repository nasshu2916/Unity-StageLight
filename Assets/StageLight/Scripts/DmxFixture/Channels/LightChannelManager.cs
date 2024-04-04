using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLight.DmxFixture.Channels
{
    public class LightChannelManager : MonoBehaviour
    {
        [SerializeField] private List<Light> _lights = new();
        [SerializeField] private float _lightIntensityCoefficient = 1;
        [SerializeField] private List<SyncMaterial> _syncMaterials = new();

        public Color32 Color32 { get; set; } = new(0, 0, 0, 0);
        public float Intensity { get; set; } = 1;
        public float? Angle { get; set; }

        [ContextMenu("Init")]
        public void Init()
        {
            _lights = GetComponentsInChildren<Light>().ToList();
        }

        public void Reset()
        {
            Init();
        }

        public void Start()
        {
            foreach (var syncMaterial in _syncMaterials.Where(syncMaterial => syncMaterial != null))
            {
                syncMaterial.Init();
            }
        }

        private void Update()
        {
            UpdateLight();
            UpdateMaterial();
        }

        private void UpdateLight()
        {
            var intensity = Intensity * _lightIntensityCoefficient;
            foreach (var light in _lights.Where(l => l != null))
            {
                light.color = Color32;
                light.intensity = intensity;
                if (Angle.HasValue) light.spotAngle = Angle.Value;
            }
        }

        private void UpdateMaterial()
        {
            foreach (var syncMaterial in _syncMaterials.Where(syncMaterial => syncMaterial != null))
            {
                syncMaterial.UpdateMaterials(Color32, Intensity);
            }
        }
    }
}

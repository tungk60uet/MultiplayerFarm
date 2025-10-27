using System;
using Lean.Pool;
using UnityEngine;

namespace _Project.Scripts
{
    public class FarmField : MonoBehaviour
    {
        private string cropType;
        private CropConfig cropConfig;
        private long seededAt;
        private int currentStep;
        // Public properties to indicate whether this field can be seeded or harvested
        public bool SeedAble => string.IsNullOrEmpty(cropType);
        public bool HarvestAble => !string.IsNullOrEmpty(cropType) && currentStep >= 3;
        private void UpdateVisuals()
        {
            if(transform.childCount>0)
                LeanPool.Despawn(transform.GetChild(0).gameObject);
            if (string.IsNullOrEmpty(cropType)) return;
            var prefabName = $"Crop/Farm_Crop_{cropType}_Step_0{currentStep}";
            var prefab = Resources.Load<Transform>(prefabName);
            var graphicTransform=LeanPool.Spawn(prefab, transform);
            graphicTransform.localPosition=new Vector3(0,2,0);
        }
        private void FixedUpdate()
        {
            if (string.IsNullOrEmpty(cropType)) return;
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var elapsedSecond = (now - seededAt) / 1000f;
            var nextStep = Mathf.Clamp(Mathf.CeilToInt(elapsedSecond / (cropConfig.growthTime / 3)), 1, 3);
            if (nextStep != currentStep)
            {
                currentStep = nextStep;
                UpdateVisuals();
            }
        }
        public void Seed(string newCropType)
        {
            if (!SeedAble) return;
            this.cropType = newCropType;
            this.cropConfig= Config.Instance.crops.Find(c => c.cropName == newCropType);
            this.currentStep = 0;
            this.seededAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public void Harvest()
        {
            if (!HarvestAble) return;
            cropType = null;
            UpdateVisuals();
        }

       

      
    }
}
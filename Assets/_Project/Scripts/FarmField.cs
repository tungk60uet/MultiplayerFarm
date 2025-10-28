using System;
using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Lean.Pool;
using UnityEngine;

namespace _Project.Scripts
{
    public class FarmField : NetworkBehaviour
    {
        private readonly SyncVar<string> cropType=new SyncVar<string>();
        
        private CropConfig _cropConfig;
        private CropConfig cropConfig
        {
            get
            {
                if (string.IsNullOrEmpty(cropType.Value)) return null;
                if (_cropConfig?.cropName != cropType.Value)
                    _cropConfig = Config.Instance.crops.Find(c => c.cropName == cropType.Value);
                return _cropConfig;
            }
        }
        private readonly SyncVar<long> seededAt=new SyncVar<long>();

        private int currentStep;
        // Public properties to indicate whether this field can be seeded or harvested
        public bool SeedAble => string.IsNullOrEmpty(cropType.Value);
        public bool HarvestAble => !string.IsNullOrEmpty(cropType.Value) && currentStep >= 3;
        private void UpdateVisuals()
        {
            if (transform.childCount > 0)
                for (var i = transform.childCount - 1; i >= 0; i--)
                    LeanPool.Despawn(transform.GetChild(i).gameObject);
            if (string.IsNullOrEmpty(cropType.Value)) return;
            var prefabName = $"Crops/Farm_Crop_{cropType.Value}_Step_0{currentStep}";
            Debug.Log($"Updating visuals for crop {cropType.Value} at step {currentStep} using prefab {prefabName}");
            var prefab = Resources.Load<Transform>(prefabName);
            var graphicTransform=LeanPool.Spawn(prefab, transform);
            graphicTransform.localPosition=new Vector3(0,2,0);
            if (currentStep==3)
            {
                // Add sickle tool visual for harvesting
                var sicklePrefab=Resources.Load<Transform>("Props/Farm_Tool_Sickle");
                var sickleTransform = LeanPool.Spawn(sicklePrefab, transform);
                sickleTransform.DOKill();
                sickleTransform.localPosition=new Vector3(0,3,0);
                sickleTransform.DOScale(2, 0.5f).From(0).SetEase(Ease.OutBack);
                sickleTransform.DORotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360)
                    .From(Vector3.zero)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental);
            }
        }
        private void FixedUpdate()
        {
            if (string.IsNullOrEmpty(cropType.Value))
            {
                if(currentStep!=0)
                {
                    currentStep = 0;
                    UpdateVisuals();
                }
                return;
            }
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var elapsedSecond = (now - seededAt.Value) / 1000f;
            var nextStep = Mathf.Clamp(Mathf.CeilToInt(elapsedSecond / (cropConfig.growthTime / 3)), 1, 3);
            if (nextStep != currentStep)
            {
                currentStep = nextStep;
                UpdateVisuals();
            }
        }
        [ServerRpc(RequireOwnership = false)]
        public void Seed(string newCropType)
        {
            if (!SeedAble) return;
            this.cropType.Value = newCropType;
            this.seededAt.Value = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        [ServerRpc(RequireOwnership = false)]
        public void Harvest()
        {
            if (!HarvestAble) return;
            cropType.Value = null;
        }
    }
}
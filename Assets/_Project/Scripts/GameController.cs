using System.Collections.Generic;
using FishNet.Object;
using Lean.Gui;
using UnityEngine;

namespace _Project.Scripts
{
    public class GameController : NetworkBehaviour
    {
        public static GameController Instance { get; private set; }
        [SerializeField] private FarmField[] farmField;
        [SerializeField] private PlayerController localPlayer;
        [SerializeField] private Transform indicatorTransform;
        
        [SerializeField] private LeanToggle seedToggle;
        [SerializeField] private LeanToggle harvestToggle;

        [SerializeField] private float gridSize = 2f;
        private readonly Dictionary<Vector2, FarmField> farmFieldDict = new();
        private Vector2 lastIndicatorGridPosition;

        private void Awake()
        {
            Instance = this;
            foreach (var field in farmField)
            {
                var gridPos = new Vector2(
                    Mathf.RoundToInt(field.transform.position.x),
                    Mathf.RoundToInt(field.transform.position.z)
                );
                farmFieldDict[gridPos] = field;
            }
        }

        public void SetInput(Vector2 input)
        {
            if (localPlayer != null && localPlayer.IsOwner)
                localPlayer.InputDirection = input;
        }
      

        public void SetLocalPlayer(PlayerController player)
        {
            localPlayer = player;
        }

        private void Update()
        {
            if (localPlayer == null || indicatorTransform == null) return;

            // Snap X and Z to nearest multiple of gridSize
            var snappedX = Mathf.Round(localPlayer.transform.position.x / gridSize) * gridSize;
            var snappedZ = Mathf.Round(localPlayer.transform.position.z / gridSize) * gridSize;
            var newGridPos = new Vector2(snappedX, snappedZ);

            // Update stored grid position if changed
            if (newGridPos != lastIndicatorGridPosition)
            {
                lastIndicatorGridPosition = newGridPos;
                if (farmFieldDict.TryGetValue(lastIndicatorGridPosition, out var currentField))
                {
                    seedToggle.On = currentField.SeedAble;
                    harvestToggle.On = currentField.HarvestAble;
                }
                else
                {
                    seedToggle.On = false;
                    harvestToggle.On = false;
                }
            }
            // Smoothly move indicator to the computed grid position (preserve y)
            var targetPos = new Vector3(newGridPos.x, indicatorTransform.position.y, newGridPos.y);
            indicatorTransform.position = Vector3.Lerp(indicatorTransform.position, targetPos, 0.2f);
        }
        
        public void OnClickSeed(int idxCrop)
        {
            var cropName = Config.Instance.crops[idxCrop - 1].cropName;
            if (farmFieldDict.TryGetValue(lastIndicatorGridPosition, out var currentField))
            {
                if (currentField.SeedAble)
                {
                    currentField.Seed(cropName);
                }
            }
        }

        public void OnClickHarvest()
        {
            if (farmFieldDict.TryGetValue(lastIndicatorGridPosition, out var currentField))
            {
                if (currentField.HarvestAble)
                {
                    currentField.Harvest();
                }
            }
        }
    }
}

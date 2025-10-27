using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }
        [SerializeField] private FarmField[] farmField;
        [SerializeField] private PlayerController localPlayer;
        [SerializeField] private Transform indicatorTransform;
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
                // Additional logic on grid change could go here
            }

            // Smoothly move indicator to the computed grid position (preserve y)
            var targetPos = new Vector3(newGridPos.x, indicatorTransform.position.y, newGridPos.y);
            indicatorTransform.position = Vector3.Lerp(indicatorTransform.position, targetPos, 0.2f);

            // Handle seeding and harvesting inputs
            if (farmFieldDict.TryGetValue(lastIndicatorGridPosition, out var currentField))
            {
                // Seeding logic (keys 1-6)
                for (int i = 1; i <= 6; i++)
                {
                    if (Input.GetKeyDown(i.ToString()))
                    {
                        if (currentField.SeedAble && i - 1 < Config.Instance.crops.Count)
                        {
                            var cropName = Config.Instance.crops[i - 1].cropName;
                            currentField.Seed(cropName);
                        }
                    }
                }

                // Harvesting logic (key E)
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (currentField.HarvestAble)
                    {
                        currentField.Harvest();
                    }
                }
            }
        }
    }
}

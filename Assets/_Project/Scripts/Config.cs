using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts
{
    [Serializable]
    public class CropConfig
    {
        public string cropName;
        public float growthTime;
    }
    
    [CreateAssetMenu(fileName = "Config", menuName = "SO/Config", order = 0)]
    public class Config : ScriptableObject
    {
        public List<CropConfig> crops;
        private static Config _instance;

        public static Config Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Resources.Load<Config>(nameof(Config));
                return _instance;
            }
        }
    }
}
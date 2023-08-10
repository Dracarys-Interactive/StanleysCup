using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    [CreateAssetMenu]
    public class LevelSO : ScriptableObject
    {
        public string levelName;
        public CollectableSO[] collectables;
        public int pointsToAdvance;
        public bool hasMiniMap;
        public bool useSpotLight;
    }
}

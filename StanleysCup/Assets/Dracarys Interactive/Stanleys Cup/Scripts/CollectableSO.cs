using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    [CreateAssetMenu]
    public class CollectableSO : ScriptableObject
    {
        public string collectableName;
        public GameObject prefab;
        public int maximumInstances;
        public float secondsBetweenSpawns;
        public float lifetimeInSeconds;
        public int points;
        public RectTransform spawningRect;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class SpawnableSO : ScriptableObject
    {
        public string spawnableName;
        public GameObject prefab;
        public int maximumInstances;
        public float secondsBetweenSpawns;
        public float lifetimeInSeconds;
        public RectangleSO spawningRect;
        public bool persistent;
    }
}

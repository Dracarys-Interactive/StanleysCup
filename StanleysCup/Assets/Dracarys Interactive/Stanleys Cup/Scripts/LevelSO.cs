using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    [CreateAssetMenu]
    public class LevelSO : ScriptableObject
    {
        [TextArea(2, 100)]
        public string levelName;
        public CollectableSO[] collectables;
        public PlatformSO[] platforms;
        public EnemySO[] enemies;
        public int pointsToAdvance;
        public bool hasMiniMap;
        public bool useSpotLight;
        public int lives;
        public LevelSO prevLevel;
        public LevelSO nextLevel;
        public bool canDoubleJump;
        public bool isTutorial = false;
        public Rect playerSpawningRect;
    }
}

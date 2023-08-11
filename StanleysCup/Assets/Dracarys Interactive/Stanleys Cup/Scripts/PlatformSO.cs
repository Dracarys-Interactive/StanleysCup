using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    [CreateAssetMenu]
    public class PlatformSO : SpawnableSO
    {
        public enum Movement
        {
            up,
            down,
            left,
            right,
            waypoints
        }

        public Movement movement = Movement.up;
        public Vector2[] waypoints;
    }
}

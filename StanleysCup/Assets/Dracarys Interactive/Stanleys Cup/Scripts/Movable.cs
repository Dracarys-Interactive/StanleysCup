using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class Moveable : MonoBehaviour
    {
        public float speed = 1f;
        public float speedVariance = .2f;
        public bool yaxis = true;
        public bool isFixed = false;
        public Vector2[] waypoints;
        public int currentWaypoint = 0;
        public bool moving = true;

        void Start()
        {
            speed += speedVariance < 0 ? Random.Range(speedVariance, 0) : Random.Range(0, speedVariance);

            if (!isFixed)
            {
                if (Random.value < .5)
                    speed *= -1;

                if (Random.value < .5)
                    yaxis = false;
            }

            if (waypoints != null && waypoints.Length > 0)
            {
                transform.position = waypoints[currentWaypoint = 0];
            }

        }

        void Update()
        {
            if (waypoints != null && waypoints.Length > 0)
                WaypointMovement();
            else
            {
                Vector2 position = transform.position;

                if (yaxis)
                    position.y += speed * Time.deltaTime;
                else
                    position.x += speed * Time.deltaTime;

                transform.position = position;
            }
        }

        void WaypointMovement()
        {
            // if there isn't anything in My_Waypoints
            if (waypoints.Length != 0)
            {

                // move towards waypoint
                transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint], speed * Time.deltaTime);

                // if the enemy is close enough to waypoint, make it's new target the next waypoint
                if (Vector3.Distance(waypoints[currentWaypoint], transform.position) <= 0)
                {
                    currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                }
            }
        }
    }
}

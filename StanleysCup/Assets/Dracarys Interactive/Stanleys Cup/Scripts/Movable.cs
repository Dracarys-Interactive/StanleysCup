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

        void Awake()
        {
            speed += speedVariance < 0 ? Random.Range(speedVariance, 0) : Random.Range(0, speedVariance);

            if (!isFixed)
            {
                if (Random.value < .5)
                    speed *= -1;

                if (Random.value < .5)
                    yaxis = false;
            }
        }

        void Update()
        {
            Vector2 position = transform.position;

            if (yaxis)
                position.y += speed * Time.deltaTime;
            else
                position.x += speed * Time.deltaTime;

            transform.position = position;
        }
    }
}

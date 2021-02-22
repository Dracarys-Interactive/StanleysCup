using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float speed = 1f;
    public bool yaxis = true;

    void Awake()
    {
        if (Random.value < .5)
            speed *= -1;

        if (Random.value < .5)
            yaxis = false;
    }

    void Update()
    {
        Vector2 position = transform.position;
        if(yaxis) 
            position.y += speed * Time.deltaTime;
        else
            position.x += speed * Time.deltaTime;
        transform.position = position;
    }
}

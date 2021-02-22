using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsDetector : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {

        switch(collision.tag)
        {
            case "HockeyStickPlatform":
                Destroy(collision.gameObject);
                break;
            default:
                break;
        }
    }
}

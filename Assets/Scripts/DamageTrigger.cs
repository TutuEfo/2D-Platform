using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // This method will be called, when something collides with "Trigger" game object.

        /*  One way to trigger it is that by using "Tag" method, but it is not that efficient if we use "Tag" for different purpose:
        
            if (collision.tag == "Player")
            {
                Debug.Log("Player entered trigger!");
            }
   
        */

        Player player = collision.gameObject.GetComponent<Player>();

        // Same as if (player != NULL)
        player?.Knockback();
        

    }
}

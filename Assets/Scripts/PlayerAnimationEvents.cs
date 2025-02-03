using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    // Only we can add an animation event to an object if the animator and the script is in the same place, since we have Player as a parent
    // of the Animator we can't add an animation event so we create this script to help us with that.

    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void FinishRespawn() => player.RespawnFinished(true);
    

}

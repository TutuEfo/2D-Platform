using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // We write it like this because we only get the component one time not every frame.
    // We write in Player.cs the other way because we want to to get the component every frame.
    private Animator anim => GetComponent<Animator>();
    private bool active;

    [SerializeField] private bool canBeReactivated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active && canBeReactivated == false)
        {
            return;
        }

        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        active = true;

        anim.SetTrigger("activate");

        GameManager.instance.UpdateRespawnPosition(transform);
    }
}

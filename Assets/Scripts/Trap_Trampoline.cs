using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Trampoline : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private float pushPower;
    // [SerializeField] private Vector2 pushDirection;
    // default value for the duration is .5f
    [SerializeField] private float duration;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.Push(transform.up * pushPower,duration);
            anim.SetTrigger("activate");
        }
    }
}

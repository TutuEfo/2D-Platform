using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Plant : Enemy
{
    [Header("Plant Details")]
    public float lastTimeAttacked;

    protected override void Update()
    {
        base.Update();

        if (isPlayerDetected)
        {
            Attack();
        }
    }

    private void Attack()
    {
        lastTimeAttacked = Time.time;
        anim.SetTrigger("attack");
    }

    protected override void HandleAnimator()
    {
        // Keep it empty unless you want to update the parameters.
    }
}

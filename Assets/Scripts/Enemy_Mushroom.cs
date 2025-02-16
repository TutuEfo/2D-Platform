using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mushroom : Enemy
{
    private BoxCollider2D cd;

    protected override void Awake()
    {
        base.Awake();

        cd = GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        base.Update();

        anim.SetFloat("xVelocity", rb.velocity.x);

        if (isDead)
        {
            return;
        }

        HandleCollision();
        HandleMovement();

        if (isGrounded)
        {
            HandleTurnAround();
        }
    }

    private void HandleTurnAround()
    {
        if (!isGroundInFrontDetected || isWallDetected)
        {
            Flip();
            idleTimer = idleDuration;
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if (idleTimer > 0)
        {
            return;
        }

        rb.velocity = new Vector2(moveSpeed * facingDir, rb.velocity.y);
    }

    public override void Die()
    {
        base.Die();

        cd.enabled = false;
    }
}

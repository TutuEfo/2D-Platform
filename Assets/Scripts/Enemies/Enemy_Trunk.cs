using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Trunk : Enemy
{
    [Header("Trunk Details")]
    [SerializeField] private Enemy_Bullet bulletPrefab;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private float bulletSpeed = 10;
    [SerializeField] private float attackCooldown = 1;
    private float lastTimeAttacked;

    protected override void Update()
    {
        base.Update();

        if (isDead)
        {
            return;
        }

        bool canAttack = Time.time > lastTimeAttacked + attackCooldown;

        if (isPlayerDetected && canAttack)
        {
            Attack();
        }

        HandleMovement();

        if (isGrounded)
        {
            HandleTurnAround();
        }
    }

    private void Attack()
    {
        // Once it detecs the Player, it stops then shoots, that's why we use this line.
        idleTimer = idleDuration + attackCooldown;
        
        lastTimeAttacked = Time.time;
        anim.SetTrigger("attack");
    }

    private void CreateBullet()
    {
        Enemy_Bullet newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.identity);

        // Vector2 bulletSpeed = new Vector2(facingDir * this.bulletSpeed, 0);
        // You can use this method, if you want to use the same named variable.
        Vector2 bulletVelocity = new Vector2(facingDir * bulletSpeed, 0);
        newBullet.SetVelocity(bulletVelocity);

        if (facingDir == 1)
        {
            newBullet.FlipSprite();
        }

        // Incase, the bullet fly of the map.
        Destroy(newBullet.gameObject, 10);
    }

    private void HandleTurnAround()
    {
        if (!isGroundInFrontDetected || isWallDetected)
        {
            Flip();
            idleTimer = idleDuration;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if (idleTimer > 0)
        {
            return;
        }

        rb.linearVelocity = new Vector2(moveSpeed * facingDir, rb.linearVelocity.y);
    }
}

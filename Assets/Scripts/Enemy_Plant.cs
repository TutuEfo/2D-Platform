using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Plant : Enemy
{
    [Header("Plant Details")]
    [SerializeField] private Enemy_Bullet bulletPrefab;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private float bulletSpeed = 10;
    [SerializeField] private float attackCooldown = 1;
    private float lastTimeAttacked;

    protected override void Update()
    {
        base.Update();

        bool canAttack = Time.time > lastTimeAttacked + attackCooldown;

        if (isPlayerDetected && canAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
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

        // Incase, the bullet fly of the map.
        Destroy(newBullet.gameObject, 10);
    }

    protected override void HandleAnimator()
    {
        // Keep it empty unless you want to update the parameters.
    }
}

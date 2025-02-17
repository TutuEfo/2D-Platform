using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Enemy : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;

    [SerializeField] protected GameObject damageTrigger;
    [Space]
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float idleDuration;
    protected float idleTimer;

    [Header("Death Details")]
    [SerializeField] private float deathImpact;
    [SerializeField] private float deathRotationSpeed;
    private int deathRotationDirection = 1;
    protected bool isDead;

    [Header("Basic Collision")]
    [SerializeField] protected float groundCheckDistance = 1.1f;
    [SerializeField] protected float wallCheckDistance = .7f;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected Transform groundCheck;
    protected bool isGrounded;
    protected bool isWallDetected;
    protected bool isGroundInFrontDetected;

    protected int facingDir = -1;
    protected bool facingRight = false;

    // This helps us override "Awake" function in other enemy scripts.
    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    

    protected virtual void HandleFlip(float xValue)
    {
        if (xValue < 0 && facingRight || xValue > 0 && !facingRight)
        {
            Flip();
        }
    }

    protected virtual void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    protected virtual void Update()
    {
        idleTimer -= Time.deltaTime;

        if (isDead)
        {
            HandleDeathRotation();
        }
    }

    public virtual void Die()
    {
        damageTrigger.SetActive(false);
        anim.SetTrigger("hit");
        rb.velocity = new Vector2(rb.velocity.x, deathImpact);
        isDead = true;

        if (Random.Range(0,100) < 50)
        {
            deathRotationDirection = deathRotationDirection * -1;
        }
    }

    private void HandleDeathRotation()
    {
        transform.Rotate(0, 0, (deathRotationSpeed * deathRotationDirection) * Time.deltaTime);
    }

    protected virtual void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isGroundInFrontDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
    }
}

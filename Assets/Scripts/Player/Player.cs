using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D cd;

    private bool canBeControlled = false;

    // You can only use header, if there is visible below it so you need to use "public" or "[SerializeField] private"
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private float defaultGravityScale;
    private bool canDoubleJump;

    [Header("Buffer & Coyote Jump")]
    [SerializeField] private float bufferJumpWindow = .25f;
    private float bufferJumpActivated = -1;
    [SerializeField] private float coyoteJumpWindow = .5f;
    private float coyoteJumpActivated = -1;

    [Header("Wall Interactions")]
    [SerializeField] private float wallJumpDuration = .6f;
    [SerializeField] private Vector2 wallJumpForce;
    private bool isWallJumping;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 1;
    [SerializeField] private Vector2 knockbackPower;
    private bool isKnocked;

    [Header("Collision")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    /// <summary>
    /// LayerMask helps us to get the list of layers, so that we can work with <see cref="isGrounded"/>
    /// </summary
    [SerializeField] private LayerMask whatIsGround;
    [Space]
    [SerializeField] private Transform enemyCheck;
    [SerializeField] private float enemyCheckRadius;
    [SerializeField] private LayerMask whatIsEnemy;
    private bool isGrounded;
    private bool isAirbone;
    private bool isWallDetected;

    private float xInput;
    private float yInput;

    private bool facingRight = true;
    private int facingDirection = 1;

    [Header("VFX")]
    [SerializeField] private GameObject deathVfx;
 
    private void Awake()
    {
        // Since we made the Rigidbody2D private, we use this to get the component of the rigid body to the player automatically.
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        defaultGravityScale = rb.gravityScale;
        RespawnFinished(false);
    }

    private void Update()
    {
        // You can use "Debug.Log("Message");" to display your message on the console. 

        UpdateAirborneStatus();
        
        if (canBeControlled == false)
        {
            HandleAnimations();
            HandleCollision();
            return;
        }

        if (isKnocked)
        {
            return;
        }

        HandleEnemyDetection();
        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision();
        HandleAnimations();
    }

    private void HandleEnemyDetection()
    {
        if (rb.velocity.y >= 0)
        {
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyCheck.position, enemyCheckRadius, whatIsEnemy);

        foreach (var enemy in colliders)
        {
            Enemy newEnemy = enemy.GetComponent<Enemy>();

            if (newEnemy != null)
            {
                newEnemy.Die();
                Jump();
            }
        }
    }

    public void RespawnFinished(bool finished)
    {
        if (finished)
        {
            rb.gravityScale = defaultGravityScale;
            canBeControlled = true;
            cd.enabled = true;
        }
        else
        {
            rb.gravityScale = 0;
            canBeControlled = false;
            // To not get knockbacked when spawned.
            cd.enabled = false;
        }
    }

    public void Knockback(float sourceDamageXPosition)
    {
        float knockbackDir = 1;

        if (transform.position.x < sourceDamageXPosition)
        {
            knockbackDir = -1;
        }

        // Can't be knocked twice.
        if (isKnocked)
        {
            return;
        }

        StartCoroutine(KnockbackRoutine());
        rb.velocity = new Vector2(knockbackPower.x * knockbackDir, knockbackPower.y);
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;
        anim.SetBool("isKnocked", true);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
        anim.SetBool("isKnocked", false);
    }

    public void Die()
    {
        GameObject newFx = Instantiate(deathVfx,transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Push(Vector2 direction, float duration = 0)
    {
        StartCoroutine(PushCoroutine(direction, duration));
    }

    private IEnumerator PushCoroutine(Vector2 direction, float duration)
    {
        canBeControlled = false;

        rb.velocity = Vector2.zero;
        // Adds instant velocity to the player.
        rb.AddForce(direction, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        canBeControlled = true;
    }

    private void UpdateAirborneStatus()
    {
        // if isGrounded true, isAirborne should be false, so if that "if statement" executed that means isAirborne "true", but should be "false".
        if (isGrounded && isAirbone)
        {
            HandleLanding();
        }

        // if isGrounded false, isAirborne should be true, so if that "if statement" executed that means isAirborne "false", but should be "true".
        if (!isGrounded && !isAirbone)
        {
            BecomeAirborne();
        }
    }

    private void BecomeAirborne()
    {
        isAirbone = true;

        if (rb.velocity.y < 0)
        {
            ActivateCoyoteJump();
        }
    }

    private void HandleLanding()
    {
        isAirbone = false;
        canDoubleJump = true;

        AttemptBufferJump();
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
            RequestBufferJump();
        }
    }

    #region Buffer & Coyote Jump
    private void RequestBufferJump()
    {
        if (isAirbone)
        {
            // Time.time keep track of the ingame time, and store that time in the bufferJumpPressed.
            bufferJumpActivated = Time.time;
        }
    }

    private void AttemptBufferJump()
    {
        if (Time.time < bufferJumpActivated + bufferJumpWindow)
        {
            bufferJumpActivated = Time.time - 1;
            Jump();
        }
    }

    // You can make functions which has 1 line of code like the examples below.
    private void ActivateCoyoteJump() => coyoteJumpActivated = Time.time;

    private void CancelCoyoteJump() => coyoteJumpActivated = Time.time - 1;
    #endregion

    private void JumpButton()
    {
        bool coyoteJumpAvailable = Time.time < coyoteJumpActivated + coyoteJumpWindow;

        if (isGrounded || coyoteJumpAvailable)
        {
            Jump();
        }
        else if (isWallDetected && !isGrounded)
        {
            WallJump();
        }
        else if (canDoubleJump)
        {
            DoubleJump();
        }

        CancelCoyoteJump();
    }

    private void Jump() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    private void DoubleJump()
    {
        isWallJumping = false;
        canDoubleJump = false;
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
    }

    private void WallJump()
    {
        canDoubleJump = true;

        rb.velocity = new Vector2(wallJumpForce.x * -facingDirection, wallJumpForce.y);
        
        Flip();

        // There would be a bug, when we delete the StopAllCoroutines, so basically it resets if there is any unfinished coroutine.
        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;

        // canFlip = false;

        yield return new WaitForSeconds(wallJumpDuration);

        // canFlip = true;

        isWallJumping = false;
    }

    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.velocity.y < 0;        // Local variable


        float yModifer = yInput < 0 ? 1 : 0.5f;

        if (canWallSlide == false)
        {
            return;
        }

        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * yModifer);
    }

    private void HandleCollision()
    {
        /// <summary>
        /// Raycast can't be seen with human eyes so we need to draw a line which has the exact same parameters.
        /// So we use <<see cref="OnDrawGizmos"/>> 
        /// </summary>
        

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        // Since right and left are different directions we multiply it with facingDirection (right is 1, left is -1)
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
    }

    private void HandleAnimations()
    {
        // Other method for Animation:
        // It is getting the "isRunning" bool on the animator page and then,
        // checks the speed in order to animate "playerIdle" or "playerMove" animation.
        // anim.SetBool("isRunning", rb.velocity.x != 0);


        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }

    private void HandleMovement()
    {
        /// <summary>
        /// It is a 2D game, so number after the Vector indicates the dimension of the Vector.
        /// <see cref="rb"/> represent the rigid body of the player 
        /// so we can access the variables that we see on the Unity Inspector
        /// </summary>

        if (isWallDetected)
        {
            return;
        }

        if (isWallJumping)
        {
            return;
        }

        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void HandleFlip()
    {
        /// <summary>
        /// This function and <see cref="Flip"/> are working together
        /// We use "xInput" not "rb.velocity.x" because we need to stop moving when we run into a wall. 
        /// So we use the users input rather than the speed.
        /// </summary>
        if (xInput < 0 && facingRight || xInput > 0 && !facingRight)
        {
            Flip();
        }
    }

    private void Flip() 
    {
        facingDirection = facingDirection * -1;
        transform.Rotate(0, 180, 0);
        // If facingRight "true" become false, if facingRight "false", become true.
        facingRight = !facingRight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(enemyCheck.position, enemyCheckRadius);
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDirection), transform.position.y));
    }
}

/// <summary>
/// ===========================================================Teoric Explanations=====================================================================
/// 
/// 
/// 1) Functions and Explanations:
/// 
/// "Awake" is called first, and runs only once. You can do general setup for the game object in this function.
/// "Start" is called before the first frame update, and runs only once. You can do reset value, initialize variables, set some default, and something
/// "FixedUpdate" is called 50 times per second. You can do phyiscal calculations, and things you want to be stable/even.
/// "Update" is called once per frame. **240 FPS -> Called 240 times per second** Checking input/collision/animation updates and so much more.
/// 
/// 
/// 2) Access Types:
/// 
/// 2.1) public: The member is accessible from both within the class and from other classes, prefere use it when it is really necessary. 
/// You can see it on Unity Inspector if it is public.
/// 
/// 2.2) private: The member is only accessible within the same class and not from outside, you can't see it on Unity Inspector.
/// 
/// If you want to see on Unity Inspector write "[SerializeField]" infront of it.
/// --> Note: Make things Private as much as possible because there will be lots of things on Unity Inspector.
/// 
/// 
/// 3) Rigid Body Types:
/// 
/// 3.1) Dynamic Rigid Body: This rigid body can move and physics are applied to this rigid body.
/// --> For Example: Player, Enemies.
///     
/// 3.2) Kinematic Rigid Body: This rigid body can move but physics are not applied to this rigid body.
/// --> For Example:
///     
/// 3.3) Static Rigid Body: This rigid body can not move, and physics are not apllied to this rigid body.    
/// --> For Example: Ground.
/// 
/// 
/// 4) Blend Tree Mechanism:
/// 
/// Blend Tree helps us to animate objects based on their threshold values, which we can assign each of the animation then get a random variable from
/// the computer to animate that animation. In our case, it is "fruits"
/// 
/// </summary>


/// <summary>
/// ===========================================================Quality of Life Tricks==================================================================
/// 
/// 
/// 1) Easier if-else format:
/// 
/// if (yInput < 0)
/// {
/// yModifer = 1; 
/// }
/// else
/// {
/// yModifer = 0.5f;
/// }
/// 
/// if-else above is equal to "yInput < 0 ? 1 : 0.5f"
/// 
/// [Explanation below]:
/// "?" is asking the question "is yInput < 0"
/// if the answer is yes yModifer = 1
/// if the answer is no yModifer = 0.5f
/// 
/// 2) Changing the name of the function from everywhere:
/// 
/// if you want to change your function name you can press, ctrl + R + R and rename it anything you want. (Refactor.Rename)
/// 
/// 
/// </summary>
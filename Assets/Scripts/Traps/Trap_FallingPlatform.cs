using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_FallingPlatform : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D[] colliders;


    [SerializeField] private float speed = .75f;
    [SerializeField] private float travelDistance;
    private Vector3[] wayPoints;
    private int wayPointIndex;
    private bool canMove = false;

    [Header("Platfor Fall Details")]
    [SerializeField] private float impactSpeed = 3;
    [SerializeField] private float impactDuration = .1f;
    private float impactTimer;
    private bool impactHappened;
    [Space]
    [SerializeField] private float fallDelay = .5f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<BoxCollider2D>();
    }

    private void Start()
    {
        SetupWayPoints();
        float randomDelay = Random.Range(0, .6f);
        Invoke(nameof(ActivatePlatform), randomDelay);
    }

    /// Alternative Version:
    /// private Ienumerator Start()
    /// {
    /// SetupWayPoints();
    /// float randomDelay = Random.Range(0, .6f);
    /// 
    /// yield return new WaitForSeconds(randomDelay)
    /// 
    /// canMove = true;

    private void ActivatePlatform() => canMove = true;

    private void SetupWayPoints()
    {
        wayPoints = new Vector3[2];

        float yOffset = travelDistance / 2;

        wayPoints[0] = transform.position + new Vector3(0, yOffset, 0);
        wayPoints[1] = transform.position + new Vector3(0, -yOffset, 0);
    }

    private void Update()
    {
        HandleImpact();
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (canMove == false)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[wayPointIndex], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPoints[wayPointIndex]) < 0.1f)
        {
            wayPointIndex++;

            if (wayPointIndex >= wayPoints.Length)
            {
                wayPointIndex = 0;
            }
        }
    }

    private void HandleImpact()
    {
        if (impactTimer < 0)
        {
            return;
        }

        impactTimer -= Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3.down * 10), impactSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (impactHappened)
        {
            return;
        }

        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            // Alternative but not so usefull, because we need to change name manually: Invoke("SwitchOffPlatform", fallDelay);
            Invoke(nameof(SwitchOffPlatform), fallDelay);
            impactTimer = impactDuration;
            impactHappened = true;
        }
    }

    private void SwitchOffPlatform()
    {
        anim.SetTrigger("deactivate");

        canMove = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3.5f;
        // For a smoother fall experience.
        rb.linearDamping = .5f;

        foreach (BoxCollider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }
}

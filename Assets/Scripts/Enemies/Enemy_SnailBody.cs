using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SnailBody : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private float zRotation;

    public void SetupBody(float yVelocity, float zRotation, int facingDirection)
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, yVelocity);

        // this.zRotation refers to the private float zRotation
        this.zRotation = zRotation;

        if (facingDirection == 1)
        {
            sr.flipX = true;
        }
    }

    private void Update()
    {
        transform.Rotate(0, 0, zRotation * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SnailBody : MonoBehaviour
{
    private Rigidbody2D rb;
    private float zRotation;

    public void SetupBody(float yVelocity, float zRotation)
    {
        rb.GetComponent<Rigidbody2D>();

        rb.velocity = new Vector2(rb.velocity.x, yVelocity);

        // this.zRotation refers to the private float zRotation
        this.zRotation = zRotation;
    }

    private void Update()
    {
        transform.Rotate(0, 0, zRotation * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackFlower : MonoBehaviour
{
    private Rigidbody2D rb;
    public float jumpForce;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void JumpAttack()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        rb.gravityScale = 0;
    }

    public void Fall()
    {
        rb.gravityScale = 3;
    }
}
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float inputX;
    private bool isGround;
    private IPlayer player;

    [Header("Player 参数")]
    public float speed;

    public float jumpForce;
    public LayerMask layer;
    public float footOffset;
    public float groundDistance;
    public int jumpCount = 1;
    public float JumpMutiplier = 1.5f;
    public float fallMutiplier = 3.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        CheckedGround();
        Jump();
    }

    private void FixedUpdate()
    {
        Move();
        ChangeGraivity();
    }

    #region 检测地面

    private void CheckedGround()
    {
        RaycastHit2D leftHit = Raycast(new Vector2(-footOffset, -0.5f), Vector2.down, groundDistance, layer);
        RaycastHit2D rightHit = Raycast(new Vector2(footOffset, -0.5f), Vector2.down, groundDistance, layer);
        if (leftHit || rightHit)
        {
            isGround = true;
            jumpCount = 1;
        }
        else
        {
            isGround = false;
        }
    }

    #endregion 检测地面

    #region 射线检测

    private RaycastHit2D Raycast(Vector2 offset, Vector2 direction, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, direction, length, layer);
        Color color = hit ? Color.red : Color.green;
        Debug.DrawRay(pos + offset, direction * length, color);
        return hit;
    }

    #endregion 射线检测

    #region 跳跃重力修改

    private void ChangeGraivity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMutiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (JumpMutiplier - 1) * Time.deltaTime;
        }
    }

    #endregion 跳跃重力修改

    #region 移动

    private void Move()
    {
        rb.velocity = new Vector2(inputX * speed, rb.velocity.y);
    }

    #endregion 移动

    #region 跳跃

    private void Jump()
    {
        var currentGraivity = rb;
        if (Input.GetButtonDown("Jump") && isGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else if (Input.GetButtonDown("Jump") && !isGround && jumpCount > 0)
        {
            jumpCount--;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    #endregion 跳跃

    private void Sprint()
    {
        player.Sprint(rb);
    }
}
using Cinemachine.Examples;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float inputX;
    private bool isGround;
    private float speed;
    private Animator anim;
    private int maxHealth = 4;
    private int currentHealth;

    [Header("Player ����")]
    public float walkSpeed;

    public float runSpeed;
    public float jumpForce;
    public LayerMask layer;
    public float footOffset;
    public float groundDistance;
    public int jumpCount = 1;
    public float JumpMutiplier = 1.5f;
    public float fallMutiplier = 3.0f;
    public float attackSpeed;
    public float coolTime;
    public float lastTimer = -1f;
    public Image cdImage;
    public GameObject skillText;
    public Image healthBar;
    public Image skillImage;
    public Sprite skillIcon;
    public float hitSpeed = 1.0f;
    public Vector2 hitDirection;
    private float lerpSpeed = 3;
    public GameObject playerB;

    private bool canJump;
    private bool isAttack;
    private bool longAttack;
    private bool isHit;
    [SerializeField] private bool isTalk;
    [SerializeField] private bool isDead;
    private TimeLine timeLine;

    private void Awake()
    {
        currentHealth = maxHealth;
        timeLine = GameObject.Find("TimeLineFather").GetComponent<TimeLine>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDead) return;
        inputX = Input.GetAxis("Horizontal");
        speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        CheckedGround();
        if (canJump)
        {
            Jump();
        }
        Attack();
        Flip();
        //ChangeCDImage();
        //ChangeHealthBar();
    }

    private void FixedUpdate()
    {
        if (isDead && isTalk)
        {
            return;
        }
        Move();
        ChangeGraivity();
    }

    private void ChangeCDImage()
    {
        cdImage.fillAmount -= 1.0f / coolTime * Time.deltaTime;
    }

    private void ChangeHealthBar()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (currentHealth / maxHealth), lerpSpeed * Time.deltaTime);
    }

    private void Attack()
    {
        if (Input.GetButtonDown("Fire1") && !isAttack)
        {
            isAttack = true;
            anim.SetTrigger("isAttack");
        }
        if (Input.GetButtonDown("Fire2") && longAttack)
        {
            if (Time.time >= (lastTimer + coolTime))
            {
                lastTimer = Time.time;
                anim.SetTrigger("longAttack");
                cdImage.fillAmount = 1;
            }
        }
    }

    public void ChangeAttack()
    {
        isAttack = false;
    }

    #region ������

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

    #endregion ������

    #region ���߼��

    private RaycastHit2D Raycast(Vector2 offset, Vector2 direction, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, direction, length, layer);
        Color color = hit ? Color.red : Color.green;
        Debug.DrawRay(pos + offset, direction * length, color);
        return hit;
    }

    #endregion ���߼��

    #region ��Ծ�����޸�

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

    #endregion ��Ծ�����޸�

    #region �ƶ�

    private void Move()
    {
        if (isAttack)
        {
            rb.velocity = new Vector2(transform.localScale.x * attackSpeed, rb.velocity.y);
        }
        else if (isHit)
        {
            rb.velocity = -hitDirection * hitSpeed;
            isHit = false;
        }
        else
        {
            rb.velocity = new Vector2(inputX * speed, rb.velocity.y);
        }
        anim.SetBool("isGround", isGround);
        anim.SetFloat("Horizontal", rb.velocity.x);
    }

    private void Flip()
    {
        if (rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (rb.velocity.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    #endregion �ƶ�

    #region ��Ծ

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

    #endregion ��Ծ

    public void SetJump()
    {
        canJump = true;
        StartCoroutine(SetTextActive());
    }

    public void SetLongAttack()
    {
        longAttack = true;
        skillImage.sprite = skillIcon;
        playerB.SetActive(true);

        playerB.transform.position = gameObject.transform.position;
        playerB.transform.rotation = gameObject.transform.rotation;
        gameObject.SetActive(false);
    }

    private IEnumerator SetTextActive()
    {
        skillText.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        skillText.SetActive(false);
    }

    public void Damage(Vector2 direction)
    {
        isHit = true;
        currentHealth -= 1;
        hitDirection = direction;
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            isDead = true;
            timeLine.SetIsDead();
            anim.SetTrigger("isDead");
            //Destroy(gameObject);
            gameObject.SetActive(false);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void Damage(int damage)
    {
        isHit = true;
        currentHealth -= damage;
        //hitDirection = direction;
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            isDead = true;
            timeLine.SetIsDead();
            //Destroy(gameObject);
            gameObject.SetActive(false);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void SetIsTalkTrue()
    {
        isTalk = true;
    }

    public void SetIsTalkFalse()
    {
        isTalk = false;
    }

    public bool GetIsDead()
    {
        return isDead;
    }
}
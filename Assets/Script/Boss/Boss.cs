using System;
using System.Collections.Generic;
using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

public enum BossType
{
    Idle, ChargeAttack, BigAttack, Hit, Death
}

// �½�һ�������õ��˵Ĳ��������ڹ���
[Serializable]
public class BossParmeter
{
    [Header("���˵�״̬")]
    public int health; // ����

    public float moveSpeed; //�ƶ��ٶ�
    public float chaseSpeed; // ׷���ٶ�
    public float chargeTime; // ֹͣʱ��
    public float bigTime; // ֹͣʱ��
    public Transform[] patrolPoints; // Ѳ�߷�Χ
    public Transform[] chasePoints; // ׷����Χ
    public Animator animator; // ����������
    public Transform target; // ׷��Ŀ��
    public LayerMask targetLayer; // Ŀ��㼶
    public Transform attackPoint; // Բ�ļ��λ��
    public Transform chargePoint; // Բ�ļ��λ��
    public Transform bigPoint; // Բ�ļ��λ��
    public float attackRadius; // Բ�İ뾶
    public bool getHit;
    public float hitSpeed;
    public int chaseHit;
    public int bigHit;
    public int damage;
    public int count = 0;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Animator hitAnimation;
}

public class Boss : MonoBehaviour
{
    // ��unity����п����������
    public BossParmeter parameter;

    public GameObject bulletPrefab;
    public Transform bigMuzzle;
    public Transform chargeMuzzle;

    private IState currentState; // ��ǰ״̬ ֱ���ýӿ����� ��̬

    // ʹ���ֵ�ע������״̬  ��ö��������Ϊ�ֵ��Key
    private Dictionary<BossType, IState> states = new Dictionary<BossType, IState>();

    // Start is called before the first frame update
    private void Start()
    {
        // ע������״̬�������Լ�������ת��״̬
        states.Add(BossType.Idle, new BossIdleState(this));
        states.Add(BossType.ChargeAttack, new BossChargeAttackState(this));
        states.Add(BossType.BigAttack, new BossBigAttackState(this));
        states.Add(BossType.Hit, new BossHitState(this));
        states.Add(BossType.Death, new BossDeadState(this));

        // ʹ���л�״̬���������ó�ʼ״̬Ϊ idle
        TransitionState(BossType.Idle);

        // ��ȡ�������еĶ��������������update�г���ִ�е�ǰ״̬��onUpdate����
        parameter.animator = GetComponent<Animator>();
        //parameter.hitAnimation = transform.GetChild(2).GetComponent<Animator>();
        parameter.rb = GetComponent<Rigidbody2D>();
        parameter.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        // ����ִ��OnUpdate()
        currentState.OnUpdate();
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    parameter.getHit = true;
        //}
    }

    #region �л�״̬

    public void GetHit(int damage)
    {
        //transform.localScale = new Vector3(-direction.x, 1, 1);
        //parameter.direction = direction;
        parameter.getHit = true;
        parameter.damage = damage;
    }

    public void Death()
    {
        Destroy(gameObject, 2F);
    }

    public void TransitionState(BossType type)
    {
        // �Ȱ�ǰһ��״̬�˳���ִ��OnExit
        if (currentState != null)
        {
            currentState.OnExit();
        }
        // ����ǰ״̬�л�Ϊ����״̬,ͨ���ֵ��key�ҵ�ö���е�״̬��
        currentState = states[type];
        // ������״̬��ִ��OnEnter();
        currentState.OnEnter();
    }

    #endregion �л�״̬

    /* #region ���ĳ���

     public void FlipTo(Transform target)
     {
         // �жϳ����Ŀ�겻Ϊ��
         if (target != null)
         {
             // �ж�������˵�x���������Ŀ��X�����꣬������ת
             if (transform.position.x > target.position.x)
             {
                 transform.localScale = new Vector3(-1, 1, 1);
             }
             // �ж�������˵�x������С��Ŀ��X�����꣬�����ҷ�ת
             else if (transform.position.x < target.position.x)
             {
                 transform.localScale = new Vector3(1, 1, 1);
             }
         }
     }

     #endregion ���ĳ���*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �жϽ������������ײ��㼶�Ƿ�ʹPlayer
        if (collision.CompareTag("Player"))
        {
            Debug.Log(collision.tag);
            // ����ǣ����Player��λ�ø���target
            parameter.target = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log(collision.tag);
            // ����˳����߷�Χʱ����target��Ϊ��
            parameter.target = null;
        }
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    // �жϽ������������ײ��㼶�Ƿ�ʹPlayer
    //    if (collision.CompareTag("Player"))
    //    {
    //        //Debug.Log(collision.tag);
    //        // ����ǣ����Player��λ�ø���target
    //        parameter.target = collision.transform;
    //    }
    //}

    #region ��������Ļ�ϻ���һЩͼ��ĺ���

    private void OnDrawGizmos()
    {
        // ����һ������Բ(Բ�ģ��뾶����ɫ)
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackRadius);
        //Gizmos.DrawWireCube(parameter.attackPoint.position, new Vector3(20, 2, 1));
    }

    #endregion ��������Ļ�ϻ���һЩͼ��ĺ���

    public void ChargeAttack()
    {
        var direction = (new Vector2(parameter.target.position.x, parameter.target.position.y) - new Vector2(transform.position.x, transform.position.y)).normalized;
        GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
        bullet.transform.position = chargeMuzzle.position;
        bullet.transform.localRotation = transform.localRotation;
        bullet.GetComponent<Bullet>().SetSpeed(direction);
    }

    public void BigAttack()
    {
        var direction = (new Vector2(parameter.target.position.x, parameter.target.position.y) - new Vector2(transform.position.x, transform.position.y)).normalized;
        GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
        bullet.transform.position = bigMuzzle.position;
        bullet.transform.localRotation = transform.localRotation;
        bullet.GetComponent<Bullet>().SetSpeed(direction);
    }
}
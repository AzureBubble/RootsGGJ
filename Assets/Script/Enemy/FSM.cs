using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

public enum EnemyState
{
    Idle, Patrol, Chase, React, Attack, Hit, Death
}

// �½�һ�������õ��˵Ĳ��������ڹ���
[Serializable]
public class EnemyParmeter
{
    [Header("���˵�״̬")]
    public int health; // ����

    public float moveSpeed; //�ƶ��ٶ�
    public float chaseSpeed; // ׷���ٶ�
    public float idleTime; // ֹͣʱ��
    public Transform[] patrolPoints; // Ѳ�߷�Χ
    public Transform[] chasePoints; // ׷����Χ
    public Animator animator; // ����������
    public Transform target; // ׷��Ŀ��
    public LayerMask targetLayer; // Ŀ��㼶
    public Transform attackPoint; // Բ�ļ��λ��
    public float attackRadius; // Բ�İ뾶
    public bool getHit;
    public Vector2 direction;
    public float hitSpeed;
    public Rigidbody2D rb;
    public Animator hitAnimation;
    public int damage;
}

public class FSM : MonoBehaviour
{
    // ��unity����п����������
    public EnemyParmeter parameter;

    private IState currentState; // ��ǰ״̬ ֱ���ýӿ����� ��̬

    // ʹ���ֵ�ע������״̬  ��ö��������Ϊ�ֵ��Key
    private Dictionary<EnemyState, IState> states = new Dictionary<EnemyState, IState>();

    // Start is called before the first frame update
    private void Start()
    {
        // ע������״̬�������Լ�������ת��״̬
        states.Add(EnemyState.Idle, new IdleState(this));
        states.Add(EnemyState.Patrol, new patrolState(this));
        states.Add(EnemyState.Attack, new attackState(this));
        states.Add(EnemyState.Chase, new chaseState(this));
        states.Add(EnemyState.React, new reactState(this));
        states.Add(EnemyState.Hit, new hitState(this));
        states.Add(EnemyState.Death, new deathState(this));

        // ʹ���л�״̬���������ó�ʼ״̬Ϊ idle
        TransitionState(EnemyState.Idle);

        // ��ȡ�������еĶ��������������update�г���ִ�е�ǰ״̬��onUpdate����
        parameter.animator = GetComponent<Animator>();
        parameter.hitAnimation = transform.GetChild(2).GetComponent<Animator>();
        parameter.rb = GetComponent<Rigidbody2D>();
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

    public void GetHit(Vector2 direction)
    {
        transform.localScale = new Vector3(-direction.x, 1, 1);
        parameter.direction = direction;
        parameter.getHit = true;
    }

    public void GetHit(Vector2 direction, int damage)
    {
        transform.localScale = new Vector3(-direction.x, 1, 1);
        parameter.damage = damage;
        parameter.direction = direction;
        parameter.getHit = true;
    }

    public void TransitionState(EnemyState type)
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

    #region ���ĳ���

    public void FlipTo(Transform target)
    {
        // �жϳ����Ŀ�겻Ϊ��
        if (target != null)
        {
            // �ж�������˵�x���������Ŀ��X�����꣬������ת
            if (transform.position.x > target.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            // �ж�������˵�x������С��Ŀ��X�����꣬�����ҷ�ת
            else if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    #endregion ���ĳ���

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �жϽ������������ײ��㼶�Ƿ�ʹPlayer
        if (collision.CompareTag("Player"))
        {
            // ����ǣ����Player��λ�ø���target
            parameter.target = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ����˳����߷�Χʱ����target��Ϊ��
            parameter.target = null;
        }
    }

    #region ��������Ļ�ϻ���һЩͼ��ĺ���

    private void OnDrawGizmos()
    {
        // ����һ������Բ(Բ�ģ��뾶����ɫ)
        //Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackRadius);
    }

    #endregion ��������Ļ�ϻ���һЩͼ��ĺ���
}
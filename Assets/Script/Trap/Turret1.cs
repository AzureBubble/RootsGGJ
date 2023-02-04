using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

public enum turretType
{
    Idle, Attack, Death, Hit
}

// �½�һ�������õ��˵Ĳ��������ڹ���
[Serializable]
public class TurretParmeter
{
    [Header("���˵�״̬")]
    public Rigidbody2D rb;

    public GameObject bowPrefab;

    public GameObject target;
    public Transform muzzple;
    public float interval = 1.0f;
    public int health = 5;
    public Animator animator;

    public float timer;
    public Vector2 direction;
    public AnimatorStateInfo info;
    public bool isDead;

    public bool getHit;
}

public class Turret1 : MonoBehaviour
{
    // ��unity����п����������
    public TurretParmeter parameter;

    private IState currentState; // ��ǰ״̬ ֱ���ýӿ����� ��̬

    // ʹ���ֵ�ע������״̬  ��ö��������Ϊ�ֵ��Key
    private Dictionary<turretType, IState> states = new Dictionary<turretType, IState>();

    // Start is called before the first frame update
    private void Start()
    {
        // ע������״̬�������Լ�������ת��״̬
        states.Add(turretType.Idle, new TurretIdleState(this));
        states.Add(turretType.Attack, new TurretAttackState(this));
        states.Add(turretType.Death, new TurretHitState(this));
        states.Add(turretType.Death, new TurretDeathState(this));

        // ʹ���л�״̬���������ó�ʼ״̬Ϊ idle
        TransitionState(turretType.Idle);

        // ��ȡ�������еĶ��������������update�г���ִ�е�ǰ״̬��onUpdate����
        parameter.animator = GetComponent<Animator>();
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

    public void TransitionState(turretType type)
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
                transform.localScale = new Vector3(-1, 1, 1);
            }
            // �ж�������˵�x������С��Ŀ��X�����꣬�����ҷ�ת
            else if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
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
            parameter.target = collision.gameObject;
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
        //Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackRadius);
    }

    #endregion ��������Ļ�ϻ���һЩͼ��ĺ���
}
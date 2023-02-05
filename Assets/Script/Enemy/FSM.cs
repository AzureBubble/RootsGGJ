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

// 新建一个类设置敌人的参数，便于管理
[Serializable]
public class EnemyParmeter
{
    [Header("敌人的状态")]
    public int health; // 生命

    public float moveSpeed; //移动速度
    public float chaseSpeed; // 追击速度
    public float idleTime; // 停止时间
    public Transform[] patrolPoints; // 巡逻范围
    public Transform[] chasePoints; // 追击范围
    public Animator animator; // 动画控制器
    public Transform target; // 追击目标
    public LayerMask targetLayer; // 目标层级
    public Transform attackPoint; // 圆心检测位置
    public float attackRadius; // 圆的半径
    public bool getHit;
    public Vector2 direction;
    public float hitSpeed;
    public Rigidbody2D rb;
    public Animator hitAnimation;
    public int damage;
}

public class FSM : MonoBehaviour
{
    // 在unity面板中看到该类参数
    public EnemyParmeter parameter;

    private IState currentState; // 当前状态 直接用接口声明 多态

    // 使用字典注册所有状态  把枚举类型作为字典的Key
    private Dictionary<EnemyState, IState> states = new Dictionary<EnemyState, IState>();

    // Start is called before the first frame update
    private void Start()
    {
        // 注册所有状态，并把自己的引用转给状态
        states.Add(EnemyState.Idle, new IdleState(this));
        states.Add(EnemyState.Patrol, new patrolState(this));
        states.Add(EnemyState.Attack, new attackState(this));
        states.Add(EnemyState.Chase, new chaseState(this));
        states.Add(EnemyState.React, new reactState(this));
        states.Add(EnemyState.Hit, new hitState(this));
        states.Add(EnemyState.Death, new deathState(this));

        // 使用切换状态函数，设置初始状态为 idle
        TransitionState(EnemyState.Idle);

        // 获取到属性中的动画器组件，并在update中持续执行当前状态的onUpdate函数
        parameter.animator = GetComponent<Animator>();
        parameter.hitAnimation = transform.GetChild(2).GetComponent<Animator>();
        parameter.rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        // 持续执行OnUpdate()
        currentState.OnUpdate();
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    parameter.getHit = true;
        //}
    }

    #region 切换状态

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
        // 先把前一个状态退出，执行OnExit
        if (currentState != null)
        {
            currentState.OnExit();
        }
        // 将当前状态切换为给定状态,通过字典的key找到枚举中的状态类
        currentState = states[type];
        // 进入新状态，执行OnEnter();
        currentState.OnEnter();
    }

    #endregion 切换状态

    #region 更改朝向

    public void FlipTo(Transform target)
    {
        // 判断朝向的目标不为空
        if (target != null)
        {
            // 判断如果敌人的x轴坐标大于目标X轴坐标，则向左翻转
            if (transform.position.x > target.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            // 判断如果敌人的x轴坐标小于目标X轴坐标，则向右翻转
            else if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    #endregion 更改朝向

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 判断进入敌人视线碰撞体层级是否使Player
        if (collision.CompareTag("Player"))
        {
            // 如果是，则把Player的位置给到target
            parameter.target = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 玩家退出视线范围时，把target置为空
            parameter.target = null;
        }
    }

    #region 可以在屏幕上绘制一些图像的函数

    private void OnDrawGizmos()
    {
        // 绘制一个空心圆(圆心，半径，颜色)
        //Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackRadius);
    }

    #endregion 可以在屏幕上绘制一些图像的函数
}
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

// 新建一个类设置敌人的参数，便于管理
[Serializable]
public class BossParmeter
{
    [Header("敌人的状态")]
    public int health; // 生命

    public float moveSpeed; //移动速度
    public float chaseSpeed; // 追击速度
    public float chargeTime; // 停止时间
    public float bigTime; // 停止时间
    public Transform[] patrolPoints; // 巡逻范围
    public Transform[] chasePoints; // 追击范围
    public Animator animator; // 动画控制器
    public Transform target; // 追击目标
    public LayerMask targetLayer; // 目标层级
    public Transform attackPoint; // 圆心检测位置
    public Transform chargePoint; // 圆心检测位置
    public Transform bigPoint; // 圆心检测位置
    public float attackRadius; // 圆的半径
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
    // 在unity面板中看到该类参数
    public BossParmeter parameter;

    public GameObject bulletPrefab;
    public Transform bigMuzzle;
    public Transform chargeMuzzle;

    private IState currentState; // 当前状态 直接用接口声明 多态

    // 使用字典注册所有状态  把枚举类型作为字典的Key
    private Dictionary<BossType, IState> states = new Dictionary<BossType, IState>();

    // Start is called before the first frame update
    private void Start()
    {
        // 注册所有状态，并把自己的引用转给状态
        states.Add(BossType.Idle, new BossIdleState(this));
        states.Add(BossType.ChargeAttack, new BossChargeAttackState(this));
        states.Add(BossType.BigAttack, new BossBigAttackState(this));
        states.Add(BossType.Hit, new BossHitState(this));
        states.Add(BossType.Death, new BossDeadState(this));

        // 使用切换状态函数，设置初始状态为 idle
        TransitionState(BossType.Idle);

        // 获取到属性中的动画器组件，并在update中持续执行当前状态的onUpdate函数
        parameter.animator = GetComponent<Animator>();
        //parameter.hitAnimation = transform.GetChild(2).GetComponent<Animator>();
        parameter.rb = GetComponent<Rigidbody2D>();
        parameter.spriteRenderer = GetComponent<SpriteRenderer>();
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

    /* #region 更改朝向

     public void FlipTo(Transform target)
     {
         // 判断朝向的目标不为空
         if (target != null)
         {
             // 判断如果敌人的x轴坐标大于目标X轴坐标，则向左翻转
             if (transform.position.x > target.position.x)
             {
                 transform.localScale = new Vector3(-1, 1, 1);
             }
             // 判断如果敌人的x轴坐标小于目标X轴坐标，则向右翻转
             else if (transform.position.x < target.position.x)
             {
                 transform.localScale = new Vector3(1, 1, 1);
             }
         }
     }

     #endregion 更改朝向*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 判断进入敌人视线碰撞体层级是否使Player
        if (collision.CompareTag("Player"))
        {
            Debug.Log(collision.tag);
            // 如果是，则把Player的位置给到target
            parameter.target = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log(collision.tag);
            // 玩家退出视线范围时，把target置为空
            parameter.target = null;
        }
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    // 判断进入敌人视线碰撞体层级是否使Player
    //    if (collision.CompareTag("Player"))
    //    {
    //        //Debug.Log(collision.tag);
    //        // 如果是，则把Player的位置给到target
    //        parameter.target = collision.transform;
    //    }
    //}

    #region 可以在屏幕上绘制一些图像的函数

    private void OnDrawGizmos()
    {
        // 绘制一个空心圆(圆心，半径，颜色)
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackRadius);
        //Gizmos.DrawWireCube(parameter.attackPoint.position, new Vector3(20, 2, 1));
    }

    #endregion 可以在屏幕上绘制一些图像的函数

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    // 添加状态机的引用
    private FSM manager;

    // 添加属性对象，获取属性设置
    private EnemyParmeter parameter;

    // 观察时间计时器
    private float timer = 0;

    // 在构造函数中获取状态机对象并通过状态机对象获取属性
    public IdleState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        // 播放idle动画
        parameter.animator.Play("idle");
    }

    public void OnUpdate()
    {
        // 每帧增加计时器的时间
        timer += Time.deltaTime;
        if (parameter.getHit)
        {
            manager.TransitionState(EnemyState.Hit);
        }

        // 发现玩家的时候,且玩家的位置在巡逻追击范围内,切换状态为反应状态
        if (parameter.target != null
            && parameter.target.position.x >= parameter.chasePoints[0].position.x
            && parameter.target.position.x <= parameter.chasePoints[1].position.x)
        {
            manager.TransitionState(EnemyState.React);
        }

        // 如果计时器大于停止时间，切换状态到巡逻
        if (timer >= parameter.idleTime)
        {
            manager.TransitionState(EnemyState.Patrol);
        }
    }

    public void OnExit()
    {
        // 退出状态时候，把计时器设置为0
        timer = 0;
    }
}

public class patrolState : IState
{
    // 添加状态机的引用
    private FSM manager;

    // 添加属性对象，获取属性设置
    private EnemyParmeter parameter;

    // 用做下标查找和切换巡逻点
    private int patrolPosition;

    // 在构造函数中获取状态机对象并通过状态机对象获取属性
    public patrolState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        // 播放run动画
        parameter.animator.Play("walk");
    }

    public void OnUpdate()
    {
        // 在update()中持续的朝向巡逻点方向
        manager.FlipTo(parameter.patrolPoints[patrolPosition]);

        // 使用MoveTowards()更新敌人的位置，让敌人移动到巡逻点
        manager.transform.position = Vector2.MoveTowards
            (manager.transform.position,
            parameter.patrolPoints[patrolPosition].position,
            parameter.moveSpeed * Time.deltaTime);

        if (parameter.getHit)
        {
            manager.TransitionState(EnemyState.Hit);
        }
        Debug.Log(Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position));
        // 敌人位置与目标巡逻点的位置小于0.1f时
        if (Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position) < 1f)
        {
            // 切换状态到idle状态
            manager.TransitionState(EnemyState.Idle);
        }
        // 发现玩家的时候,且玩家的位置在巡逻追击范围内,切换状态为反应状态
        if (parameter.target != null
            && parameter.target.position.x >= parameter.chasePoints[0].position.x
            && parameter.target.position.x <= parameter.chasePoints[1].position.x)
        {
            manager.TransitionState(EnemyState.React);
        }
    }

    public void OnExit()
    {
        // 推出这个状态时，增加巡逻点的坐标，确定下一个巡逻点的坐标
        patrolPosition++;
        // 对下标进行判断是否超过巡逻点数组范围
        if (patrolPosition >= parameter.patrolPoints.Length)
        {
            // 超出则置为0
            patrolPosition = 0;
        }
    }
}

public class chaseState : IState
{
    // 添加状态机的引用
    private FSM manager;

    // 添加属性对象，获取属性设置
    private EnemyParmeter parameter;

    // 在构造函数中获取状态机对象并通过状态机对象获取属性
    public chaseState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        // 进入状态播放chase动画
        parameter.animator.Play("walk");
    }

    public void OnUpdate()
    {
        // 使敌人始终朝向追击目标
        manager.FlipTo(parameter.target);
        // 如果追击目标不为空
        if (parameter.target)
        {
            // 则使敌人使用追击速度追击目标
            manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                parameter.target.position, parameter.chaseSpeed * Time.deltaTime);
        }
        if (parameter.getHit)
        {
            manager.TransitionState(EnemyState.Hit);
        }
        // 追击目标丢失的时候||敌人超出追击距离时
        if (parameter.target == null
            || manager.transform.position.x < parameter.chasePoints[0].position.x
            || manager.transform.position.x > parameter.chasePoints[1].position.x)
        {
            // 把敌人状态切换回idle
            manager.TransitionState(EnemyState.Idle);
        }
        // 检测攻击范围(圆心位置，半径，目标图层)
        if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackRadius, parameter.targetLayer))
        {
            // 检测到玩家，则切换到攻击状态
            manager.TransitionState(EnemyState.Attack);
        }
    }

    public void OnExit()
    {
    }
}

public class reactState : IState
{
    // 添加状态机的引用
    private FSM manager;

    // 添加属性对象，获取属性设置
    private EnemyParmeter parameter;

    // 获取动画播放进度
    private AnimatorStateInfo info;

    // 在构造函数中获取状态机对象并通过状态机对象获取属性
    public reactState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        // 进入状态时，播放react动画
        parameter.animator.Play("react");
    }

    public void OnUpdate()
    {
        // 在状态进行时，让敌人朝向玩家，并且实时获取到动画状态
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        // 当动画状态接近1时候，即可认为动画播放完成
        if (info.normalizedTime >= .95f)
        {
            // 然后切换状态到追击状态
            manager.TransitionState(EnemyState.Chase);
        }
    }

    public void OnExit()
    {
    }
}

public class attackState : IState
{
    // 添加状态机的引用
    private FSM manager;

    // 添加属性对象，获取属性设置
    private EnemyParmeter parameter;

    // 获取动画播放进度
    private AnimatorStateInfo info;

    // 在构造函数中获取状态机对象并通过状态机对象获取属性
    public attackState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        // 进入状态，播放attack动画
        parameter.animator.Play("attack");
    }

    public void OnUpdate()
    {
        // 在状态进行时，让敌人朝向玩家，并且实时获取到动画状态
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if (parameter.getHit)
        {
            manager.TransitionState(EnemyState.Hit);
        }
        // 当动画状态接近1时候，即可认为动画播放完成
        if (info.normalizedTime >= .95f)
        {
            // 然后切换状态到追击状态
            manager.TransitionState(EnemyState.Chase);
        }
    }

    public void OnExit()
    {
    }
}

public class hitState : IState
{
    // 添加状态机的引用
    private FSM manager;

    // 添加属性对象，获取属性设置
    private EnemyParmeter parameter;

    private AnimatorStateInfo info;
    private AnimatorStateInfo hitInfo;

    public hitState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("hit");
        parameter.hitAnimation.SetTrigger("hit");
        parameter.health--;
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        parameter.rb.velocity = parameter.direction * parameter.hitSpeed;
        if (parameter.health <= 0)
        {
            manager.TransitionState(EnemyState.Death);
        }
        if (info.normalizedTime >= .95f)
        {
            parameter.target = GameObject.FindWithTag("Player").transform;

            manager.TransitionState(EnemyState.Chase);
        }
    }

    public void OnExit()
    {
        parameter.getHit = false;
    }
}

public class deathState : IState
{
    // 添加状态机的引用
    private FSM manager;

    // 添加属性对象，获取属性设置
    private EnemyParmeter parameter;

    private AnimatorStateInfo info;

    public deathState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("dead");
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= .95f)
        {
            manager.gameObject.SetActive(false);
        }
    }

    public void OnExit()
    {
    }
}
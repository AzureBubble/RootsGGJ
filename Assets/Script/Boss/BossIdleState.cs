using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdleState : IState
{// 添加状态机的引用
    private Boss manager;

    // 添加属性对象，获取属性设置
    private BossParmeter parameter;

    // 观察时间计时器
    private float timer = 0;

    // 在构造函数中获取状态机对象并通过状态机对象获取属性
    public BossIdleState(Boss manager)
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
            manager.TransitionState(BossType.Hit);
        }

        // 如果计时器大于停止时间，切换状态到巡逻
        if (timer >= parameter.chargeTime && parameter.target != null && parameter.count != 3)
        {
            Debug.Log(parameter.target);
            manager.TransitionState(BossType.ChargeAttack);
            parameter.count++;
        }
        if (timer >= parameter.bigTime && parameter.count == 3 && parameter.target != null)
        {
            manager.TransitionState(BossType.BigAttack);
        }
        //if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackRadius, parameter.targetLayer))
        //{
        //    // 检测到玩家，则切换到攻击状态
        //    manager.TransitionState(BossType.ChargeAttack);
        //}
    }

    public void OnExit()
    {
        // 退出状态时候，把计时器设置为0
        timer = 0;
    }
}

public class BossChargeAttackState : IState
{// 添加状态机的引用
    private Boss manager;

    // 添加属性对象，获取属性设置
    private BossParmeter parameter;

    // 获取动画播放进度
    private AnimatorStateInfo info;

    // 在构造函数中获取状态机对象并通过状态机对象获取属性
    public BossChargeAttackState(Boss manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        // 进入状态，播放attack动画
        parameter.animator.Play("ChargeAttack");
    }

    public void OnUpdate()
    {
        // 在状态进行时，让敌人朝向玩家，并且实时获取到动画状态
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if (parameter.getHit)
        {
            manager.TransitionState(BossType.Hit);
        }
        // 当动画状态接近1时候，即可认为动画播放完成
        if (info.normalizedTime >= .95f)
        {
            manager.ChargeAttack();
            // 然后切换状态到追击状态
            manager.TransitionState(BossType.Idle);
        }
    }

    public void OnExit()
    {
    }
}

public class BossBigAttackState : IState
{
    // 添加状态机的引用
    private Boss manager;

    // 添加属性对象，获取属性设置
    private BossParmeter parameter;

    // 获取动画播放进度
    private AnimatorStateInfo info;

    // 在构造函数中获取状态机对象并通过状态机对象获取属性
    public BossBigAttackState(Boss manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        // 进入状态，播放attack动画
        parameter.animator.Play("BigAttack");
    }

    public void OnUpdate()
    {
        // 在状态进行时，让敌人朝向玩家，并且实时获取到动画状态
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if (parameter.getHit)
        {
            manager.TransitionState(BossType.Hit);
        }
        // 当动画状态接近1时候，即可认为动画播放完成
        if (info.normalizedTime >= .95f)
        {
            manager.BigAttack();
            // 然后切换状态到追击状态
            manager.TransitionState(BossType.Idle);
        }
    }

    public void OnExit()
    {
        parameter.count = 0;
    }
}

public class BossHitState : IState
{// 添加状态机的引用
    private Boss manager;

    // 添加属性对象，获取属性设置
    private BossParmeter parameter;

    private AnimatorStateInfo info;
    //private AnimatorStateInfo hitInfo;

    public BossHitState(Boss manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.health -= parameter.damage;
    }

    public void OnUpdate()
    {
        //info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        //parameter.rb.velocity = parameter.direction * parameter.hitSpeed;
        if (parameter.health <= 0)
        {
            manager.TransitionState(BossType.Death);
        }
        parameter.target = GameObject.FindWithTag("Player").transform;

        manager.TransitionState(BossType.Idle);
    }

    public void OnExit()
    {
        parameter.getHit = false;
    }
}

public class BossDeadState : IState
{
    // 添加状态机的引用
    private Boss manager;

    // 添加属性对象，获取属性设置
    private BossParmeter parameter;

    private AnimatorStateInfo info;

    public BossDeadState(Boss manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        //parameter.animator.Play("dead");
        parameter.spriteRenderer.color = Color.gray;
        manager.Death();
    }

    public void OnUpdate()
    {
        //info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        //if (info.normalizedTime >= .95f)
        //{
        //    manager.gameObject.SetActive(false);
        //}
    }

    public void OnExit()
    {
    }
}
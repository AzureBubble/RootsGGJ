using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretIdleState : IState
{
    // 添加状态机的引用
    private Turret1 manager;

    // 添加属性对象，获取属性设置
    private TurretParmeter parameter;

    // 观察时间计时器
    private float timer = 0;

    // 在构造函数中获取状态机对象并通过状态机对象获取属性
    public TurretIdleState(Turret1 manager)
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
            manager.TransitionState(turretType.Hit);
        }
        if (parameter.target != null)
        {
            manager.TransitionState(turretType.Attack);
        }
    }

    public void OnExit()
    {
        // 退出状态时候，把计时器设置为0
        timer = 0;
    }
}

public class TurretAttackState : IState
{// 添加状态机的引用
    private Turret1 manager;

    // 添加属性对象，获取属性设置
    private TurretParmeter parameter;

    // 获取动画播放进度
    private AnimatorStateInfo info;

    // 在构造函数中获取状态机对象并通过状态机对象获取属性
    public TurretAttackState(Turret1 manager)
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
            manager.TransitionState(turretType.Hit);
        }
        // 当动画状态接近1时候，即可认为动画播放完成
        if (info.normalizedTime >= .95f && parameter.target == null)
        {
            // 然后切换状态到追击状态
            manager.TransitionState(turretType.Idle);
        }
    }

    public void OnExit()
    {
    }
}

public class TurretHitState : IState
{
    // 添加状态机的引用
    private Turret1 manager;

    // 添加属性对象，获取属性设置
    private TurretParmeter parameter;

    private AnimatorStateInfo info;

    public TurretHitState(Turret1 manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.health--;
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        //parameter.rb.velocity = parameter.direction * parameter.hitSpeed;
        if (parameter.health <= 0)
        {
            manager.TransitionState(turretType.Death);
        }
        if (info.normalizedTime >= .95f && parameter.target != null)
        {
            manager.TransitionState(turretType.Attack);
        }
        else if (info.normalizedTime >= .95f && parameter.target == null)
        {
            manager.TransitionState(turretType.Idle);
        }
    }

    public void OnExit()
    {
        parameter.getHit = false;
    }
}

public class TurretDeathState : IState
{ // 添加状态机的引用
    private Turret1 manager;

    // 添加属性对象，获取属性设置
    private TurretParmeter parameter;

    private AnimatorStateInfo info;

    public TurretDeathState(Turret1 manager)
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
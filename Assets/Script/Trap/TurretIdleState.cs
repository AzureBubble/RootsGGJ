using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretIdleState : IState
{
    // ���״̬��������
    private Turret1 manager;

    // ������Զ��󣬻�ȡ��������
    private TurretParmeter parameter;

    // �۲�ʱ���ʱ��
    private float timer = 0;

    // �ڹ��캯���л�ȡ״̬������ͨ��״̬�������ȡ����
    public TurretIdleState(Turret1 manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        // ����idle����
        parameter.animator.Play("idle");
    }

    public void OnUpdate()
    {
        // ÿ֡���Ӽ�ʱ����ʱ��
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
        // �˳�״̬ʱ�򣬰Ѽ�ʱ������Ϊ0
        timer = 0;
    }
}

public class TurretAttackState : IState
{// ���״̬��������
    private Turret1 manager;

    // ������Զ��󣬻�ȡ��������
    private TurretParmeter parameter;

    // ��ȡ�������Ž���
    private AnimatorStateInfo info;

    // �ڹ��캯���л�ȡ״̬������ͨ��״̬�������ȡ����
    public TurretAttackState(Turret1 manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        // ����״̬������attack����
        parameter.animator.Play("attack");
    }

    public void OnUpdate()
    {
        // ��״̬����ʱ���õ��˳�����ң�����ʵʱ��ȡ������״̬
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if (parameter.getHit)
        {
            manager.TransitionState(turretType.Hit);
        }
        // ������״̬�ӽ�1ʱ�򣬼�����Ϊ�����������
        if (info.normalizedTime >= .95f && parameter.target == null)
        {
            // Ȼ���л�״̬��׷��״̬
            manager.TransitionState(turretType.Idle);
        }
    }

    public void OnExit()
    {
    }
}

public class TurretHitState : IState
{
    // ���״̬��������
    private Turret1 manager;

    // ������Զ��󣬻�ȡ��������
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
{ // ���״̬��������
    private Turret1 manager;

    // ������Զ��󣬻�ȡ��������
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdleState : IState
{// ���״̬��������
    private Boss manager;

    // ������Զ��󣬻�ȡ��������
    private BossParmeter parameter;

    // �۲�ʱ���ʱ��
    private float timer = 0;

    // �ڹ��캯���л�ȡ״̬������ͨ��״̬�������ȡ����
    public BossIdleState(Boss manager)
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
            manager.TransitionState(BossType.Hit);
        }

        // �����ʱ������ֹͣʱ�䣬�л�״̬��Ѳ��
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
        //    // ��⵽��ң����л�������״̬
        //    manager.TransitionState(BossType.ChargeAttack);
        //}
    }

    public void OnExit()
    {
        // �˳�״̬ʱ�򣬰Ѽ�ʱ������Ϊ0
        timer = 0;
    }
}

public class BossChargeAttackState : IState
{// ���״̬��������
    private Boss manager;

    // ������Զ��󣬻�ȡ��������
    private BossParmeter parameter;

    // ��ȡ�������Ž���
    private AnimatorStateInfo info;

    // �ڹ��캯���л�ȡ״̬������ͨ��״̬�������ȡ����
    public BossChargeAttackState(Boss manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        // ����״̬������attack����
        parameter.animator.Play("ChargeAttack");
    }

    public void OnUpdate()
    {
        // ��״̬����ʱ���õ��˳�����ң�����ʵʱ��ȡ������״̬
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if (parameter.getHit)
        {
            manager.TransitionState(BossType.Hit);
        }
        // ������״̬�ӽ�1ʱ�򣬼�����Ϊ�����������
        if (info.normalizedTime >= .95f)
        {
            manager.ChargeAttack();
            // Ȼ���л�״̬��׷��״̬
            manager.TransitionState(BossType.Idle);
        }
    }

    public void OnExit()
    {
    }
}

public class BossBigAttackState : IState
{
    // ���״̬��������
    private Boss manager;

    // ������Զ��󣬻�ȡ��������
    private BossParmeter parameter;

    // ��ȡ�������Ž���
    private AnimatorStateInfo info;

    // �ڹ��캯���л�ȡ״̬������ͨ��״̬�������ȡ����
    public BossBigAttackState(Boss manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        // ����״̬������attack����
        parameter.animator.Play("BigAttack");
    }

    public void OnUpdate()
    {
        // ��״̬����ʱ���õ��˳�����ң�����ʵʱ��ȡ������״̬
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if (parameter.getHit)
        {
            manager.TransitionState(BossType.Hit);
        }
        // ������״̬�ӽ�1ʱ�򣬼�����Ϊ�����������
        if (info.normalizedTime >= .95f)
        {
            manager.BigAttack();
            // Ȼ���л�״̬��׷��״̬
            manager.TransitionState(BossType.Idle);
        }
    }

    public void OnExit()
    {
        parameter.count = 0;
    }
}

public class BossHitState : IState
{// ���״̬��������
    private Boss manager;

    // ������Զ��󣬻�ȡ��������
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
    // ���״̬��������
    private Boss manager;

    // ������Զ��󣬻�ȡ��������
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
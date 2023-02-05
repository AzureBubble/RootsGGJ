using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    // ���״̬��������
    private FSM manager;

    // ������Զ��󣬻�ȡ��������
    private EnemyParmeter parameter;

    // �۲�ʱ���ʱ��
    private float timer = 0;

    // �ڹ��캯���л�ȡ״̬������ͨ��״̬�������ȡ����
    public IdleState(FSM manager)
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
            manager.TransitionState(EnemyState.Hit);
        }

        // ������ҵ�ʱ��,����ҵ�λ����Ѳ��׷����Χ��,�л�״̬Ϊ��Ӧ״̬
        if (parameter.target != null
            && parameter.target.position.x >= parameter.chasePoints[0].position.x
            && parameter.target.position.x <= parameter.chasePoints[1].position.x)
        {
            manager.TransitionState(EnemyState.React);
        }

        // �����ʱ������ֹͣʱ�䣬�л�״̬��Ѳ��
        if (timer >= parameter.idleTime)
        {
            manager.TransitionState(EnemyState.Patrol);
        }
    }

    public void OnExit()
    {
        // �˳�״̬ʱ�򣬰Ѽ�ʱ������Ϊ0
        timer = 0;
    }
}

public class patrolState : IState
{
    // ���״̬��������
    private FSM manager;

    // ������Զ��󣬻�ȡ��������
    private EnemyParmeter parameter;

    // �����±���Һ��л�Ѳ�ߵ�
    private int patrolPosition;

    // �ڹ��캯���л�ȡ״̬������ͨ��״̬�������ȡ����
    public patrolState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        // ����run����
        parameter.animator.Play("walk");
    }

    public void OnUpdate()
    {
        // ��update()�г����ĳ���Ѳ�ߵ㷽��
        manager.FlipTo(parameter.patrolPoints[patrolPosition]);

        // ʹ��MoveTowards()���µ��˵�λ�ã��õ����ƶ���Ѳ�ߵ�
        manager.transform.position = Vector2.MoveTowards
            (manager.transform.position,
            parameter.patrolPoints[patrolPosition].position,
            parameter.moveSpeed * Time.deltaTime);

        if (parameter.getHit)
        {
            manager.TransitionState(EnemyState.Hit);
        }
        Debug.Log(Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position));
        // ����λ����Ŀ��Ѳ�ߵ��λ��С��0.1fʱ
        if (Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position) < 1f)
        {
            // �л�״̬��idle״̬
            manager.TransitionState(EnemyState.Idle);
        }
        // ������ҵ�ʱ��,����ҵ�λ����Ѳ��׷����Χ��,�л�״̬Ϊ��Ӧ״̬
        if (parameter.target != null
            && parameter.target.position.x >= parameter.chasePoints[0].position.x
            && parameter.target.position.x <= parameter.chasePoints[1].position.x)
        {
            manager.TransitionState(EnemyState.React);
        }
    }

    public void OnExit()
    {
        // �Ƴ����״̬ʱ������Ѳ�ߵ�����꣬ȷ����һ��Ѳ�ߵ������
        patrolPosition++;
        // ���±�����ж��Ƿ񳬹�Ѳ�ߵ����鷶Χ
        if (patrolPosition >= parameter.patrolPoints.Length)
        {
            // ��������Ϊ0
            patrolPosition = 0;
        }
    }
}

public class chaseState : IState
{
    // ���״̬��������
    private FSM manager;

    // ������Զ��󣬻�ȡ��������
    private EnemyParmeter parameter;

    // �ڹ��캯���л�ȡ״̬������ͨ��״̬�������ȡ����
    public chaseState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        // ����״̬����chase����
        parameter.animator.Play("walk");
    }

    public void OnUpdate()
    {
        // ʹ����ʼ�ճ���׷��Ŀ��
        manager.FlipTo(parameter.target);
        // ���׷��Ŀ�겻Ϊ��
        if (parameter.target)
        {
            // ��ʹ����ʹ��׷���ٶ�׷��Ŀ��
            manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                parameter.target.position, parameter.chaseSpeed * Time.deltaTime);
        }
        if (parameter.getHit)
        {
            manager.TransitionState(EnemyState.Hit);
        }
        // ׷��Ŀ�궪ʧ��ʱ��||���˳���׷������ʱ
        if (parameter.target == null
            || manager.transform.position.x < parameter.chasePoints[0].position.x
            || manager.transform.position.x > parameter.chasePoints[1].position.x)
        {
            // �ѵ���״̬�л���idle
            manager.TransitionState(EnemyState.Idle);
        }
        // ��⹥����Χ(Բ��λ�ã��뾶��Ŀ��ͼ��)
        if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackRadius, parameter.targetLayer))
        {
            // ��⵽��ң����л�������״̬
            manager.TransitionState(EnemyState.Attack);
        }
    }

    public void OnExit()
    {
    }
}

public class reactState : IState
{
    // ���״̬��������
    private FSM manager;

    // ������Զ��󣬻�ȡ��������
    private EnemyParmeter parameter;

    // ��ȡ�������Ž���
    private AnimatorStateInfo info;

    // �ڹ��캯���л�ȡ״̬������ͨ��״̬�������ȡ����
    public reactState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        // ����״̬ʱ������react����
        parameter.animator.Play("react");
    }

    public void OnUpdate()
    {
        // ��״̬����ʱ���õ��˳�����ң�����ʵʱ��ȡ������״̬
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        // ������״̬�ӽ�1ʱ�򣬼�����Ϊ�����������
        if (info.normalizedTime >= .95f)
        {
            // Ȼ���л�״̬��׷��״̬
            manager.TransitionState(EnemyState.Chase);
        }
    }

    public void OnExit()
    {
    }
}

public class attackState : IState
{
    // ���״̬��������
    private FSM manager;

    // ������Զ��󣬻�ȡ��������
    private EnemyParmeter parameter;

    // ��ȡ�������Ž���
    private AnimatorStateInfo info;

    // �ڹ��캯���л�ȡ״̬������ͨ��״̬�������ȡ����
    public attackState(FSM manager)
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
            manager.TransitionState(EnemyState.Hit);
        }
        // ������״̬�ӽ�1ʱ�򣬼�����Ϊ�����������
        if (info.normalizedTime >= .95f)
        {
            // Ȼ���л�״̬��׷��״̬
            manager.TransitionState(EnemyState.Chase);
        }
    }

    public void OnExit()
    {
    }
}

public class hitState : IState
{
    // ���״̬��������
    private FSM manager;

    // ������Զ��󣬻�ȡ��������
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
    // ���״̬��������
    private FSM manager;

    // ������Զ��󣬻�ȡ��������
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
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

public class Bow : MonoBehaviour
{
    private Transform target;
    public float speed;
    private Animator animator;

    public const float g = 9.8f;

    /// <summary>
    /// �ٶ�Խ����������Խ�ӽ���ˮƽ,����Ϊ0
    /// </summary>
    private float verticalSpeed;

    //private float time;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private IEnumerator move()
    {
        float time = 0;
        while (true)
        {
            time += Time.fixedDeltaTime;
            float test = verticalSpeed - g * time;
            if (target.position.x - transform.position.x > 0)
            {
                transform.Translate(speed * Time.fixedDeltaTime, 0, 0, Space.World);
            }
            else if (target.position.x - transform.position.x < 0)
            {
                transform.Translate(-speed * Time.fixedDeltaTime, 0, 0, Space.World);
            }
            transform.Translate(0, test * Time.fixedDeltaTime, 0, Space.World);
            yield return null;
        }
    }

    public void Set(Transform target)
    {
        this.target = target;
        float tmepDistance = Vector3.Distance(transform.position, target.position);
        float tempTime = tmepDistance / speed;
        float riseTime = tempTime / 2;
        verticalSpeed = g * riseTime;
        // ���ó�ʼ��ת����Ŀ��,�����Ͳ�����Ϊ��ʼ��ת����ͬ����Translate�˶�����ͬ
        transform.LookAt(target.position);
        StartCoroutine(move());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (transform.localScale.x < 0)
            {
                collision.GetComponent<PlayerMovement>().Damage(Vector2.right);
            }
            else if (transform.localScale.x > 0)
            {
                collision.GetComponent<PlayerMovement>().Damage(Vector2.left);
            }
            //collision.gameObject.GetComponent<PlayerMovement>().Damage();
            ObjectPool.Instance.PushObject(gameObject);
        }
        if (collision.CompareTag("Ground"))
        {
            ObjectPool.Instance.PushObject(gameObject);
        }
    }
}
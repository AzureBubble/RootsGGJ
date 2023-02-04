using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

public class Bow : MonoBehaviour
{
    private Transform target;
    public float speed;

    public const float g = 9.8f;

    /// <summary>
    /// 速度越大整体曲线越接近于水平,不可为0
    /// </summary>
    private float verticalSpeed;

    //private float time;

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
        // 设置初始旋转朝向目标,这样就不会因为初始旋转方向不同导致Translate运动方向不同
        //transform.LookAt(target.position);
        StartCoroutine(move());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().Damage();
            ObjectPool.Instance.PushObject(gameObject);
        }
    }
}
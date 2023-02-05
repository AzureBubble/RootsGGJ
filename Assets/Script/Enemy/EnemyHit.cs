using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Player"))
        //{
        //    if (transform.localScale.x < 0)
        //    {
        //        collision.GetComponent<PlayerMovement>().Damage(Vector2.right);
        //    }
        //    else if (transform.localScale.x > 0)
        //    {
        //        collision.GetComponent<PlayerMovement>().Damage(Vector2.left);
        //    }
        //    //collision.gameObject.GetComponent<PlayerMovement>().Damage();
        //    ObjectPool.Instance.PushObject(gameObject);
        //}
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovement>().Damage(1);
            Debug.Log(1);
        }
    }
}
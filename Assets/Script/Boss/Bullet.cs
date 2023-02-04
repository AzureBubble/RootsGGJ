using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    //public GameObject explosionPrefab;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void SetSpeed(Vector2 direction)
    {
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        //GameObject exp = ObjectPool.Instance.GetObject(explosionPrefab);
        //exp.transform.position = transform.position;
        //Destroy(gameObject);
        //ObjectPool.Instance.PushObject(gameObject);
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
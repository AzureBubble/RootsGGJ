using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turret : MonoBehaviour
{
    public GameObject bowPrefab;
    public GameObject target;
    public Transform muzzple;
    public float interval = 1.0f;
    private int health = 5;
    private Animator animator;

    public float timer;
    private Vector2 direction;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (target != null)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        direction = (new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(transform.position.x, transform.position.y)).normalized;
        //transform.right = direction;
        if (timer != 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
            }
        }
        if (timer == 0 && target != null)
        {
            Fire();
            timer = interval;
        }
    }

    private void Fire()
    {
        animator.SetTrigger("isAttack");
        GameObject bow = ObjectPool.Instance.GetObject(bowPrefab);
        bow.transform.position = muzzple.position;
        bow.transform.localRotation = transform.localRotation;
        bow.GetComponent<Bow>().Set(target.transform);
    }

    public void Damage(int damage)
    {
        health--;
        if (health <= 0)
        {
            animator.SetBool("isDead", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            target = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        target = null;
    }
}
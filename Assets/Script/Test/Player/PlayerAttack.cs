using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private int damage = 1;
    private bool canDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("turret") && canDamage)
        {
            canDamage = false;
            collision.gameObject.GetComponentInParent<turret>().Damage(damage);
            Debug.Log(damage);
            //if (transform.localScale.x > 0)
            //{
            //    collision.GetComponent<Turret1>().GetHit(Vector2.right);
            //}
            //else if (transform.localScale.x < 0)
            //{
            //    collision.GetComponent<Turret1>().GetHit(Vector2.left);
            //}
        }
        if (collision.CompareTag("Boss") && canDamage)
        {
            canDamage = false;
            collision.gameObject.GetComponentInParent<Boss>().GetHit(damage);
            Debug.Log(damage);
            //if (transform.localScale.x > 0)
            //{
            //    collision.GetComponent<Turret1>().GetHit(Vector2.right);
            //}
            //else if (transform.localScale.x < 0)
            //{
            //    collision.GetComponent<Turret1>().GetHit(Vector2.left);
            //}
        }
        if (collision.CompareTag("Enemy") && canDamage)
        {
            canDamage = false;
            if (transform.localScale.x > 0)
            {
                collision.GetComponent<FSM>().GetHit(Vector2.right);
            }
            else if (transform.localScale.x < 0)
            {
                collision.GetComponent<FSM>().GetHit(Vector2.left);
            }
            Debug.Log(damage);
            //if (transform.localScale.x > 0)
            //{
            //    collision.GetComponent<Turret1>().GetHit(Vector2.right);
            //}
            //else if (transform.localScale.x < 0)
            //{
            //    collision.GetComponent<Turret1>().GetHit(Vector2.left);
            //}
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("turret") && !canDamage)
        {
            canDamage = true;
        }
        if (collision.CompareTag("Boss") && !canDamage)
        {
            canDamage = true;
        }
        if (collision.CompareTag("Enemy") && !canDamage)
        {
            canDamage = true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackFlowerTrap : MonoBehaviour
{
    private GameObject target;

    private void Update()
    {
        if (target != null)
        {
            Checked();
        }
    }

    private void Checked()
    {
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
        if (collision.CompareTag("Player"))
        {
            target = null;
        }
    }
}
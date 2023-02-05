using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Update is called once per frame
    public Transform playerObject;

    private void Update()
    {
        if (FindObjectOfType<PlayerMovement>() != null)
        {
            Vector3 tempPosition = FindObjectOfType<PlayerMovement>().transform.position;
            playerObject.transform.position = tempPosition;
        }
    }
}
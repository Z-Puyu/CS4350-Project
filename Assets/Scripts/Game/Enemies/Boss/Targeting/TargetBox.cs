using System;
using UnityEngine;

public class TargetBox : MonoBehaviour
{
    private Transform playerTarget;
    public void FollowPlayer(Transform playerTransform)
    {
        playerTransform = playerTransform;
    }

    void Update()
    {
        if (playerTarget != null)
        {
            transform.position = playerTarget.position;   
        }
    }
}

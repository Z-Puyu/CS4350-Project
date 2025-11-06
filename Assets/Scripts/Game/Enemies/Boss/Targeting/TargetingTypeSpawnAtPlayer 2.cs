using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetTypeSpawnAtPlayer", menuName = "Target type/Spawn at player")]
public class TargetingTypeSpawnAtPlayer : TargetingType
{
    private GameObject playerGameObject;
    private void Start()
    {
        playerGameObject = GameObject.FindGameObjectWithTag("Player");
    }

    public override void SpawnTarget(TargetBox box)
    {
        TargetBox targetBox = Instantiate(box, playerGameObject.transform);
        targetBox.FollowPlayer(playerGameObject.transform);
    }
}

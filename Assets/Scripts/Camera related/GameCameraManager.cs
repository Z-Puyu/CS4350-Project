using Unity.Cinemachine;
using UnityEngine;

public class GameCameraManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineCamera virtualCamera;
    
    void Awake()
    {
        player = GameObject.Find("Player");
        virtualCamera.Follow = player.transform;
    }
}

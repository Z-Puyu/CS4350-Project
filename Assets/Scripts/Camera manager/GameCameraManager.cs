using Unity.Cinemachine;
using UnityEngine;

namespace Camera_manager {
    public class GameCameraManager : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private CinemachineCamera virtualCamera;
    
        void Awake()
        {
            this.player = GameObject.Find("Player");
            this.virtualCamera.Follow = this.player.transform;
        }
    }
}
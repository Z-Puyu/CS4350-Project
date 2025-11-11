using Unity.Cinemachine;
using UnityEngine;

namespace Camera_manager {
    public class GameCameraManager : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private CinemachineCamera virtualCamera;
        private CinemachineConfiner2D confiner;
    
        void Awake()
        {
            this.player = GameObject.Find("Player");
            this.virtualCamera.Follow = this.player.transform;
            confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
        }

        public void SetConfinerCollider(PolygonCollider2D collider)
        {
            confiner.BoundingShape2D = collider;
        }
    }
}
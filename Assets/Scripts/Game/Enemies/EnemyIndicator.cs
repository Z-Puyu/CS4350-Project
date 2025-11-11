using UnityEngine;
using Events;
using Game.Enemies;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Enemies {
    public class EnemyIndicator : MonoBehaviour
    {
        protected Enemy enemy;
        protected Camera camera;
        protected Image image;
        public CrossObjectEventSO fetchNewEnemyToTarget;
        protected RectTransform rectTransform;
        [SerializeField] private float borderSize;
        [SerializeField] private float angleOffset;
        
        protected void Start()
        {
            image = GetComponent<Image>();
            camera = Camera.main;
            rectTransform = GetComponent<RectTransform>();
        }
        
        public void SetTargetEnemy(Component component, object none)
        {
            enemy = (Enemy)component;
        }

        public void CheckIfTargetedEnemyIsDead(Component component, object none)
        {
            if (enemy == (Enemy)component)
            {
                fetchNewEnemyToTarget.TriggerEvent();
            }
        }

        protected void Update()
        {
            UpdatePosition();
        }

        protected void UpdatePosition()
        {
            if (enemy.IsUnityNull())
            {
                image.enabled = false;
                return;
            }
            Vector3 fromPosition = camera.transform.position;
            fromPosition.z = 0;
            Vector3 direction = (enemy.transform.position - fromPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            rectTransform.localEulerAngles = new Vector3(0, 0, angle - angleOffset);
            
            Vector3 enemyPositionToScreenPosition = camera.WorldToScreenPoint(enemy.transform.position);
            bool isOffscreen = enemyPositionToScreenPosition.x <= borderSize || enemyPositionToScreenPosition.x >= Screen.width - borderSize||
                               enemyPositionToScreenPosition.y <= borderSize|| enemyPositionToScreenPosition.y >= Screen.height - borderSize;
            if (isOffscreen)
            {
                image.enabled = true;
                if (enemyPositionToScreenPosition.x <= borderSize) enemyPositionToScreenPosition.x = borderSize;
                if (enemyPositionToScreenPosition.x >= Screen.width - borderSize) enemyPositionToScreenPosition.x = Screen.width - borderSize;
                if (enemyPositionToScreenPosition.y <= borderSize) enemyPositionToScreenPosition.y = borderSize;
                if (enemyPositionToScreenPosition.y >= Screen.height - borderSize) enemyPositionToScreenPosition.y = Screen.height - borderSize;

                rectTransform.position = camera.ScreenToWorldPoint(enemyPositionToScreenPosition);
                rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0);
            }
            else
            {
                image.enabled = false;
            }
        }
    }
}
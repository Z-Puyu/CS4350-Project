using GameplayAbilities.Runtime.HealthSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Enemies
{
    public class EnemyUIManager : MonoBehaviour
    {
        public Slider healthBar;
        public Health health;
        
        public void UpdateHealth()
        {
            healthBar.value = (float)health.Value/(float)health.MaxValue;
        }
    }
}

using System;
using Events;
using UnityEngine;
using UnityEngine.UI;

namespace Player_related.Player {
    [Obsolete]
    public class PlayerStats : MonoBehaviour
    {
        private int currentHealth;
        [SerializeField]
        private int maxHealth;
        public CrossObjectEventWithDataSO broadcastHealth;
        public CrossObjectEventSO broadcastPlayerDie;

        void Start()
        {
            this.currentHealth = this.maxHealth;
            this.broadcastHealth.TriggerEvent(this);
        }

        public void UpdateHealthBar(Slider slider)
        {
            slider.value = ((float)this.currentHealth) / ((float)this.maxHealth);
        }

        public void ReceiveDamage(int damage)
        {
            this.currentHealth -= damage;
            this.broadcastHealth.TriggerEvent(this);
            if (this.currentHealth == 0)
            {
                this.broadcastPlayerDie.TriggerEvent();
                Destroy(this.gameObject);
            }
        }

    }
}

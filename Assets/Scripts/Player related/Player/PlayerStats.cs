using UnityEngine;
using UnityEngine.UI;
using Events;

public class PlayerStats : MonoBehaviour
{
    private int currentHealth;
    [SerializeField]
    private int maxHealth;
    public CrossObjectEventWithDataSO broadcastHealth;
    public CrossObjectEventSO broadcastPlayerDie;

    void Start()
    {
        currentHealth = maxHealth;
        broadcastHealth.TriggerEvent(this);
    }

    public void UpdateHealthBar(Slider slider)
    {
        slider.value = ((float)currentHealth) / ((float)maxHealth);
    }

    public void ReceiveDamage(int damage)
    {
        currentHealth -= damage;
        broadcastHealth.TriggerEvent(this);
        if (currentHealth == 0)
        {
            broadcastPlayerDie.TriggerEvent();
            Destroy(gameObject);
        }
    }

}

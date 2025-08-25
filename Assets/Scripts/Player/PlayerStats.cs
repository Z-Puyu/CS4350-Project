using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    private int currentHealth;
    [SerializeField]
    private int maxHealth;
    public CrossObjectEventWithDataSO broadcastHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void UpdateHealthBar(Slider slider)
    {
        slider.value = ((float)currentHealth) / ((float)maxHealth);
    }

    public void ReceiveDamage(int damage)
    {
        currentHealth -= damage;
        broadcastHealth.TriggerEvent(this);
    }

}

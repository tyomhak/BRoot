using System;
using UnityEngine;

public class Health : MonoBehaviour, IHealth
{
    [SerializeField] int _maxHealth;

    [Tooltip("Set to -1 for starting at Max Health")]
    [SerializeField] int _startingHealth = -1;

    int _health;

    public delegate void HealthEvent(Health entityHealth);
    public event HealthEvent OnHealthDepleted;

    public void TakeDamage(int amount)
    {
        SetHealth(_health - amount);

        if (GetHealth() == 0)
            OnHealthDepleted?.Invoke(this);
    }

    public void Heal(int amount) => SetHealth(_health + amount);
    public void ResetHealth() => SetHealth(_maxHealth);


    public int GetHealth() => _health;
    public int GetHealthMax() => _maxHealth;

    void SetHealth(int healthAmount)
    {
        _health = Math.Clamp(healthAmount, 0, _maxHealth);
    }
}

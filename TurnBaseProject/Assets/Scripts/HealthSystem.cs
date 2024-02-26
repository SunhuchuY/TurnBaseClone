using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDead;
    public event EventHandler OnDamaged;

    [SerializeField] private int health = 100;

    private int maxHealth;

    private void Awake()
    {
        maxHealth = health;
    }

    public void Damage(int damageAmount)
    {
        health -= damageAmount;
        health = Mathf.Clamp(health, 0, int.MaxValue);

        OnDamaged?.Invoke(this, EventArgs.Empty);

        if (health == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized() => (float)health / (float)maxHealth;
}

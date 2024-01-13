using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 0;
    [SerializeField] GameObject destructionParticles;
    [SerializeField] Image healthBar;


    public virtual void Start()
    {
        currentHealth = maxHealth;
        if (healthBar)
        {
            UpdateHealthBar(maxHealth, currentHealth);
        }
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (healthBar)
        {
            UpdateHealthBar(maxHealth, currentHealth);
        }
        if (currentHealth <= 0) {
            if (destructionParticles)
            {
                Destroy(Instantiate(destructionParticles, transform.position, Quaternion.identity), 5f);

            }
            Destroy(this.gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        healthBar.fillAmount = currentHealth/maxHealth;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthComponent : HealthComponent
{
    
    public override void Start()
    {
        base.maxHealth = 300;
        base.Start();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

    }

    protected override void OnDestroy()
    {

        base.OnDestroy();
    }
}

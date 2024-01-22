using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthComponent : HealthComponent
{
    //Ref do menadzera gry
    public override void Start()
    {
        base.Start();
    }


    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.Instance.addEnemyAlive(-1);
        GameManager.Instance.addKills(1);
    }
}

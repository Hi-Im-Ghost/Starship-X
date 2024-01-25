using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
    private float timeLeft;

    [System.Obsolete]
    public void Awake()
    {
        ParticleSystem particleTime = GetComponent<ParticleSystem>();
        timeLeft = particleTime.startLifetime + 5f;
        Destroy(this.gameObject, timeLeft);
    }

}



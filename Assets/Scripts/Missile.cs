using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("Missile Settings")]
    [SerializeField] LayerMask shootableMask;
    [SerializeField] ParticleSystem impactParticles;
    [SerializeField] AudioClip impactSound;
    [SerializeField] float missileDamage = 20f;

    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        HandleAudio();
    }

    void HandleAudio()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 50f;
        audioSource.maxDistance = 200f;
        audioSource.playOnAwake = false;
        audioSource.clip = impactSound;
    }

    void ApplyDamage(HealthComponent healthComponent)
    {
        healthComponent.TakeDamage(missileDamage);
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (shootableMask == (shootableMask | (1 << collision.gameObject.layer)))
        {   
            if (impactParticles)
            {
                Instantiate(impactParticles, collision.ClosestPoint(transform.position), Quaternion.identity);
            }
            if (impactSound)
            {
                audioSource.Play();
            }
            if (collision.gameObject.GetComponentInParent<HealthComponent>())
            {
                ApplyDamage(collision.gameObject.GetComponentInParent<HealthComponent>());

            }else if(collision.gameObject.CompareTag("Base"))
            {
                GameManager.Instance.addBarrierBase(-missileDamage);
            }
            Destroy(gameObject);
        }  
    }
}

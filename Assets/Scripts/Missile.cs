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

    private void OnCollisionEnter(Collision collision)
    {

        if (shootableMask == (shootableMask | (1 << collision.gameObject.layer)))
        {
            if (impactParticles)
            {
                Destroy(Instantiate(impactParticles, collision.contacts[0].point, Quaternion.identity),2f);
            }

            if (impactSound)
            {
                audioSource.Play();
            }
            if (collision.gameObject.GetComponent<HealthComponent>())
            {
                ApplyDamage(collision.gameObject.GetComponent<HealthComponent>());
            }

            Destroy(gameObject);
        
        }
    }
}

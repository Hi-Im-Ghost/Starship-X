using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("Missile Settings")]
    [SerializeField] LayerMask shootableMask;
    [SerializeField] ParticleSystem impactParticles;
    [SerializeField] AudioClip impactSound;


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

    private void OnCollisionEnter(Collision collision)
    {

        if (shootableMask == (shootableMask | (1 << collision.gameObject.layer)))
        {
            if (impactParticles)
            {
                Instantiate(impactParticles, collision.contacts[0].point, Quaternion.identity);
            }

            if (impactSound)
            {
                audioSource.Play();
            }


            Destroy(collision.gameObject); //Tu zadawanie obrazen jesli to jest jakis wrog
            Destroy(gameObject);
        
        }
    }
}

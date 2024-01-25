using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Missile : MonoBehaviour
{
    [Header("Missile Settings")]
    [SerializeField] LayerMask shootableMask;
    [SerializeField] ParticleSystem impactParticles;
    [SerializeField] GameObject impactSound;
    [SerializeField] float missileDamage = 20f;

    private GameObject shooter;
    void Start()
    {


    }

    public void SetShooter(GameObject shooterObject)
    {
        shooter = shooterObject;
    }


    void ApplyDamage(HealthComponent healthComponent)
    {
        healthComponent.TakeDamage(missileDamage);
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (shooter.tag != collision.tag)
        {
            if (shootableMask == (shootableMask | (1 << collision.gameObject.layer)))
            {
                if (impactParticles)
                {
                    Destroy(Instantiate(impactParticles, collision.ClosestPoint(transform.position), Quaternion.identity), 5f);
                }
                if (impactSound)
                {
                    Destroy(Instantiate(impactSound, transform.position, Quaternion.identity), 5f);
                }
                if (collision.gameObject.GetComponentInParent<HealthComponent>())
                {
                    ApplyDamage(collision.gameObject.GetComponentInParent<HealthComponent>());

                }
                else if (collision.gameObject.CompareTag("Base"))
                {
                    GameManager.Instance.addBarrierBase(-missileDamage);
                }
                Destroy(gameObject);

            }
        }
    }

}

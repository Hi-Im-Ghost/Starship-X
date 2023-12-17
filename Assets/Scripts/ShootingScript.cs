using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingScript : MonoBehaviour
{
    [Header("Guns Settings")]
    [SerializeField] Transform[] gunsTransform;
    [SerializeField] float shootRange = 200f;
    [SerializeField] bool targetInRange = false;
    [SerializeField] int missileAmmo = 12;

    [Header("Missile Controler Settings")]
    [SerializeField] GameObject missilePrefab;
    [SerializeField] float missileDamage = 10f;
    [SerializeField] float missileSpeed = 50f;

    public bool shooting = false;

    void FixedUpdate()
    {
        HandleShooting();
    }
    //30 min Ep4 celownik dla kilku dzialek
    //Zrobic UIManager dla ammo i boost
    void HandleShooting()
    {
        if (shooting && missileAmmo > 0)
        {
            foreach (Transform missile in gunsTransform)
            {
                ShootMissile(missile.position, missile.rotation);
                missileAmmo--;
                
            }
            shooting = false;
        }
        else
        {
            // Komunikat o braku amunicji 
            shooting = false;
        }
    }

    public void ShootMissile(Vector3 position, Quaternion rotation)
    {
        var missile = Instantiate(missilePrefab, position, rotation);
        Rigidbody missileRb = missile.GetComponent<Rigidbody>();
        missileRb.velocity = missile.transform.forward * missileSpeed;

        //StartCoroutine(DestroyAfterRange()); Dodac ze po przebyciu okreslonego dystansu jest niszczona
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        shooting = context.performed;
    }

    /*
        private IEnumerator DestroyAfterRange()
        {
            yield return new WaitForSeconds(destroyDelay);
            Destroy(gameObject);
        }*/
}

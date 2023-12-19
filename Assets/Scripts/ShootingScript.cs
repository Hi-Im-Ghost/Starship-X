using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingScript : MonoBehaviour
{
    [Header("Guns Settings")]
    [SerializeField] Transform[] gunsTransform;
    [SerializeField] Transform middlePoint;
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

        StartCoroutine(DestroyAfterRange(missile)); 
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        shooting = context.performed;
    }

    
    private IEnumerator DestroyAfterRange(GameObject toDestroy)
    {
        float elapsedDistance = 0f;
        float maxDistance = shootRange; // Maksymalny dystans jaki pokona pocisk

        while (elapsedDistance < maxDistance)
        {
            // Sprawdzenie czy obiekt wciaz istnieje
            if (toDestroy == null)
                yield break; // Wyjscie z funkcji jesli obiekt zostal juz zniszczony

            // Poczekaj jedna klatke
            yield return null;

            // Zaktualizuj odleglosc na podstawie predkosci pocisku i czasu
            elapsedDistance += toDestroy.GetComponent<Rigidbody>().velocity.magnitude * Time.deltaTime;
        }

        // Zniszcz obiekt po osi¹gnieciu maksymalnego dystansu
        Destroy(toDestroy);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;

public class ShootingScript : MonoBehaviour
{
    [Header("Guns Settings")]
    [SerializeField] Transform[] gunsTransform;
    [SerializeField] Transform middlePoint;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] float shootRange = 100f;
    [SerializeField] float reloadingTime = 1.0f;
    [SerializeField] int missileAmmo = 64;
    [SerializeField] int maxMissileAmmo = 512;

    [Header("Missile Controller Settings")]
    [SerializeField] GameObject missilePrefab;
    [SerializeField] float missileSpeed = 15f;

    bool shooting = false;
    private bool canShoot = true;
    private Camera cam;


    private void Start()
    {
        if (ammoText)
        {
            ammoText.text = missileAmmo.ToString();
        }
    }

    private void Awake()
    {
        cam = Camera.main;
    }

    void FixedUpdate()
    {
        HandleShooting();
    }

    void HandleShooting()
    {
        if (shooting && missileAmmo > 0 && canShoot)
        {
            if (Gamepad.current != null)
            {
                StartCoroutine(HandleShootingVibration());
            }
            RaycastHit hitInfo;
            Vector3 targetPosition;

            if (TargetInfo.isTargetInRange(middlePoint.transform.position, cam.transform.forward, out hitInfo, shootRange))
            {
                targetPosition = hitInfo.point;
            }
            else
            {
                // Domyslny punkt gdy raycast nie trafi w zaden punkt
                targetPosition = middlePoint.transform.position + cam.transform.forward * shootRange;
            }
            foreach (Transform missileGun in gunsTransform)
            {
                //Vector3 localHitPosition = missileGun.transform.InverseTransformPoint(targetPosition);
                Vector3 randomOffset = Random.onUnitSphere * 1.0f; // Przesuniecie o losowa wartosc na sferze o promieniu 0.5
                Vector3 adjustedTargetPosition = targetPosition + randomOffset;
                // Wystrzel pocisk w kierunku punktu 
                ShootMissile(missileGun.position, adjustedTargetPosition, missileGun.rotation);
                missileAmmo--;
                if (ammoText)
                {
                    ammoText.text = missileAmmo.ToString();
                }
            }
            shooting = false;
            canShoot = false;
            StartCoroutine(EnableShootingAfterDelay(reloadingTime));
        }
        else
        {
            shooting = false;
        }
    }

    public void ShootMissile(Vector3 startPosition, Vector3 targetPosition, Quaternion rotation)
    {
        // Oblicz kierunek
        Vector3 direction = targetPosition - startPosition;
        GameObject missile = Instantiate(missilePrefab, startPosition, rotation);
        // Przekazanie referencji do obiektu który inicjalizuje do rakiety
        Missile missileScript = missile.GetComponent<Missile>();
        if (missileScript != null)
        {
            missileScript.SetShooter(this.gameObject);
        }

        Rigidbody missileRb = missile.GetComponent<Rigidbody>();
        missileRb.AddForce(direction.normalized * missileSpeed, ForceMode.Impulse);

        StartCoroutine(DestroyAfterTime(missile, shootRange / missileSpeed));
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        shooting = context.performed;
    }

    private IEnumerator DestroyAfterTime(GameObject toDestroy, float destroyDelay)
    {
        yield return new WaitForSeconds(destroyDelay);

        if (toDestroy != null)
        {
            Destroy(toDestroy);
        }
    }
    IEnumerator HandleShootingVibration()
    {
        Gamepad.current.SetMotorSpeeds(0.9f, 0.1f);
        yield return new WaitForSeconds(0.2f); // Czas trwania wibracji 
        Gamepad.current.ResetHaptics(); // Zresetuj wibracje
    }

    IEnumerator EnableShootingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canShoot = true;  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ammo") && missileAmmo < maxMissileAmmo)
        {
            missileAmmo+=48;
            if (missileAmmo >= maxMissileAmmo)
            {
                missileAmmo = maxMissileAmmo;
            }

            if (ammoText)
            {
                ammoText.text = missileAmmo.ToString();
            }
            Destroy(other.gameObject);
        }
    }
}

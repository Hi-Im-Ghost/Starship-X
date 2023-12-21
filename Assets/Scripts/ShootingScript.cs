using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingScript : MonoBehaviour
{
    [Header("Guns Settings")]
    [SerializeField] Transform[] gunsTransform;
    [SerializeField] Transform middlePoint;
    [SerializeField] float shootRange = 100f;
    [SerializeField] int missileAmmo = 64;
    [SerializeField] int maxMissileAmmo = 512;

    [Header("Missile Controller Settings")]
    [SerializeField] GameObject missilePrefab;
    [SerializeField] float missileDamage = 10f;
    [SerializeField] float missileSpeed = 15f;

    bool shooting = false;
    private Camera cam;

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
        if (shooting && missileAmmo > 0)
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
                Vector3 randomOffset = Random.onUnitSphere * 2.0f; // Przesuniecie o losowa wartosc na sferze o promieniu 0.5
                Vector3 adjustedTargetPosition = targetPosition + randomOffset;
                // Wystrzel pocisk w kierunku punktu 
                ShootMissile(missileGun.position, adjustedTargetPosition, missileGun.rotation);
                missileAmmo--;
            }
            shooting = false;
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
        var missile = Instantiate(missilePrefab, startPosition, rotation);

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
}

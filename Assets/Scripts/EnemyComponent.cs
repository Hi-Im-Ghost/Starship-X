using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class EnemyComponent : MonoBehaviour
{
    private Rigidbody rb;
    private Transform player;
    private Transform basePlayer;
    private bool canShoot = true;

    [Header("Enemy Controller Settings")]
    [SerializeField] LayerMask layerPlayer, layerPlayerBase;
    [SerializeField] float sightRange = 200f;
    [SerializeField] float thrust = 25f; //Sila napedu 
    [SerializeField] float rotationSpeed = 5f; // rotacja
    bool playerInSightRange, playerInAttackRange, basePlayerInSightRange, basePlayerInAttackRange;

    [Header("Guns Controller Settings")]
    [SerializeField] Transform[] gunsTransform;
    [SerializeField] Transform middlePoint;
    [SerializeField] float shootRange = 100f;
    [SerializeField] float reloadingTime = 3.0f;
    [SerializeField] int missileAmmo = 64;
    [SerializeField] int maxMissileAmmo = 64;
    [Header("Weapon Controller Settings")]
    [SerializeField] GameObject missilePrefab;
    [SerializeField] float missileSpeed = 15f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        basePlayer = GameObject.FindGameObjectWithTag("Base").transform;
    }

    private void Awake()
    {
        GameManager.Instance.addEnemyAlive(1);
    }

    private void FixedUpdate()
    {
        basePlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, layerPlayerBase);
        basePlayerInAttackRange = Physics.CheckSphere(transform.position, shootRange, layerPlayerBase);

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, layerPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, shootRange, layerPlayer);

        if(!playerInSightRange && !playerInAttackRange && !basePlayerInAttackRange && !basePlayerInSightRange)
        {
            MoveToBasePlayer();
        }

        if (basePlayerInSightRange && basePlayerInAttackRange)
        {
            AttackBasePlayer();
        }
        if (playerInSightRange && !playerInAttackRange && !basePlayerInAttackRange)
        {
            ChasePlayer();
        }
        if(playerInSightRange && playerInAttackRange && !basePlayerInAttackRange)
        {
            AttackPlayer();
        }
        if(missileAmmo <= 0)
        {
            missileAmmo = maxMissileAmmo;
        }
       
    }

    private void MoveToBasePlayer()
    {
        HandleMove(basePlayer);
    }

    private void ChasePlayer()
    {
        HandleMove(player);
    }

    private void AttackPlayer()
    {
        transform.LookAt(player);
        HandleShooting();

    }

    private void AttackBasePlayer()
    {
        transform.LookAt(basePlayer);
        HandleShooting();
    }

    void HandleMove(Transform Moveto)
    {
        //transform.LookAt(Moveto);
        // Kierunek obortu
        Vector3 targetDirection = (Moveto.position - transform.position).normalized;
        // Obrot
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed));


        Vector3 targetLocation = Moveto.position - transform.position;
        float distance = targetLocation.magnitude;
        rb.AddRelativeForce(Vector3.forward * Mathf.Clamp((distance - shootRange) / 50, 0f, 1f) * thrust * 1.15f);

    }

    void HandleShooting()
    {
        if (missileAmmo > 0 && canShoot)
        {
            RaycastHit hitInfo;
            Vector3 targetPosition;

            if (TargetInfo.isTargetInRange(middlePoint.transform.position, middlePoint.transform.forward, out hitInfo, shootRange))
            {
                targetPosition = hitInfo.point;
                foreach (Transform missileGun in gunsTransform)
                {
                    //Vector3 localHitPosition = missileGun.transform.InverseTransformPoint(targetPosition);
                    Vector3 randomOffset = Random.onUnitSphere * 1.0f; // Przesuniecie o losowa wartosc na sferze o promieniu 0.5
                    Vector3 adjustedTargetPosition = targetPosition + randomOffset;
                    // Wystrzel pocisk w kierunku punktu 
                    ShootMissile(missileGun.position, adjustedTargetPosition, missileGun.rotation);
                    missileAmmo--;
                }
                canShoot = false;
                StartCoroutine(EnableShootingAfterDelay(reloadingTime));
            }
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

    IEnumerator EnableShootingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canShoot = true;
    }

    private IEnumerator DestroyAfterTime(GameObject toDestroy, float destroyDelay)
    {
        yield return new WaitForSeconds(destroyDelay);

        if (toDestroy != null)
        {
            Destroy(toDestroy);
        }
    }
}


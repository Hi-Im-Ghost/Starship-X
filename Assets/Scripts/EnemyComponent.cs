using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    [SerializeField] float torque = 75f; //Moment obrotowy
    [SerializeField] float thrust = 40f; //Sila napedu 
    private Rigidbody rb;
    private Transform player;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Awake()
    {
        GameManager.Instance.addEnemyAlive(1);
    }

    private void FixedUpdate()
    {
        transform.LookAt(player);

        Vector3 targetLocation = player.position - transform.position;
        float distance = targetLocation.magnitude;
        rb.AddRelativeForce(Vector3.forward * Mathf.Clamp((distance - 10) / 50, 0f, 1f) * thrust); //zatrzyma sie okolo 10 przed nami
    }
}

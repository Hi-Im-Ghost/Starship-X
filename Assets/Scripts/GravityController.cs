using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] float minForce = 10f;
    [SerializeField] float maxForce = 20f;
    [SerializeField] float minTorque = 10f;
    [SerializeField] float maxTorque = 20f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.AddForce(Vector3.forward * Random.Range(minForce,maxForce));
        rb.AddTorque(Vector3.forward * Random.Range(minTorque,maxTorque));
    }
}

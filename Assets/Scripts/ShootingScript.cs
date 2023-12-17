using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [Header("Guns Settings")]
    [SerializeField] Transform[] gunsTransform;
    [SerializeField] LayerMask shootableMask;
    [SerializeField] float shootRange = 200f;
    [SerializeField] bool targetInRange = false;


    [SerializeField] Renderer[] missile;
    [SerializeField] ParticleSystem particleMissile;
    [SerializeField] float damage = 10f;
    [SerializeField] int ammo = 10;
    void Start()
    {
        
    }


    void Update()
    {
        
    }
}

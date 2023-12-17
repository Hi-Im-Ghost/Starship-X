using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SpaceShipMovement : MonoBehaviour
{
    [Header("Ship Movement Settings")]
    [SerializeField] float rollTorque = 100f;
    [SerializeField] float pitchTorque = 100f;
    [SerializeField] float yawTorque = 100f;
    [SerializeField] float thrust = 50f;
    [SerializeField] float upThrust = 50f;
    [SerializeField] float StrafeThrust = 50f;

    [Header("Boost Settings")]
    [SerializeField] float maxBoostAmount = 20f;
    [SerializeField] float boostDeprecationRate = 0.1f;
    [SerializeField] float boostRechargeRate = 0.15f;
    [SerializeField] float boostMultiplier = 10f;
    public bool boosting = false;
    public float currentBoostAmount;

    [SerializeField, Range(0.001f, 0.999f)]
    float thrustGlideReduction = 0.330f;
    [SerializeField, Range(0.001f, 0.999f)]
    float upDownGlideReduction = 0.111f;
    [SerializeField, Range(0.001f, 0.999f)]
    float leftRightGlideReduction = 0.111f;
    float glide, verticalGlide, horizontalGlide = 0f;

    Rigidbody rb;

    float thrust1D;
    float upDown1D;
    float strafe1D;
    float roll1D;
    Vector2 pitchYaw;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentBoostAmount = maxBoostAmount;
    }


    void FixedUpdate()
    {
        HandleBoosting();
        HandleMove();
    }

    void HandleBoosting()
    {
        if (boosting && currentBoostAmount > 0f)
        {
            currentBoostAmount -= boostDeprecationRate;
            if(currentBoostAmount <= 0f)
            {
                boosting = false;
            }
        }
        else
        {
            if(currentBoostAmount < maxBoostAmount)
            {
                currentBoostAmount += boostRechargeRate; //przerobic na zbieranie energii
            }
        }
    }

    void HandleMove()
    {
        rb.AddRelativeTorque(Vector3.back * roll1D * rollTorque * Time.deltaTime);
        rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYaw.y, -1, 1f)* pitchTorque * Time.deltaTime);
        rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);
        
        if(thrust1D > 0.1f || thrust1D < -0.1)
        {
            float currentThrust; 
            if(boosting)
            {
                currentThrust = thrust * boostMultiplier;
            }
            else
            {
                currentThrust = thrust;
            }
            rb.AddRelativeForce(Vector3.forward * thrust1D* currentThrust * Time.deltaTime);
            glide = thrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
            glide *= thrustGlideReduction;
        }

        if (upDown1D > 0.1f || upDown1D < -0.1)
        {
            rb.AddRelativeForce(Vector3.up * upDown1D * upThrust * Time.fixedDeltaTime);
            verticalGlide = upDown1D * upThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.up * verticalGlide * Time.fixedDeltaTime);
            verticalGlide *= upDownGlideReduction;
        }

        if (strafe1D > 0.1f || strafe1D < -0.1)
        {
            rb.AddRelativeForce(Vector3.right * strafe1D * upThrust * Time.fixedDeltaTime);
            horizontalGlide = strafe1D * StrafeThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.right * horizontalGlide * Time.fixedDeltaTime);
            horizontalGlide *= leftRightGlideReduction;
        }

    }

    public void OnThrust(InputAction.CallbackContext context)
    {
        thrust1D = context.ReadValue<float>(); //-1, 0, 1
    }
    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe1D = context.ReadValue<float>();
    }
    public void OnUpDown(InputAction.CallbackContext context) 
    {
        upDown1D = context.ReadValue<float>();
    }
    public void OnRoll(InputAction.CallbackContext context)
    {
        roll1D = context.ReadValue<float>();
    }
    public void OnPitchView(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
    }
    public void OnBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
    }
}

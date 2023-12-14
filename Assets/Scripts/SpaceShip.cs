using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SpaceShip : MonoBehaviour
{
    [SerializeField] float rollTorque = 100f;
    [SerializeField] float pitchTorque = 200f;
    [SerializeField] float yawTorque = 500f;
    [SerializeField] float thrust = 100f;
    [SerializeField] float upThrust = 50f;
    [SerializeField] float StrafeThrust = 50f;

    [SerializeField, Range(0.001f, 0.999f)]
    float thrustGlideReduction = 0.5f;
    [SerializeField, Range(0.001f, 0.999f)]
    float upDownGlideReduction = 0.111f;
    [SerializeField, Range(0.001f, 0.999f)]
    float leftRightGlideReduction = 0.111f;
    float glide = 0f;

    Rigidbody rb;

    float thrust1D;
    float upDown1D;
    float strafe1D;
    float roll1D;
    Vector2 pitchYaw;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        HandleMove();
    }

    void HandleMove()
    {
        rb.AddRelativeTorque(Vector3.back * roll1D * rollTorque * Time.deltaTime);
        rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYaw.y, -1, 1f)* pitchTorque * Time.deltaTime);
        rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);
        
        if(thrust1D > 0.1f || thrust1D < -0.1)
        {
            float currentThrust = thrust;
            rb.AddRelativeForce(Vector3.forward * thrust1D* currentThrust * Time.deltaTime);
            glide = thrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
            glide *= Time.deltaTime;
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
}

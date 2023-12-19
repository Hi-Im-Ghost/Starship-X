using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SpaceShipMovement : MonoBehaviour
{
    // Ustawienia ruchu statku
    [Header("Ship Movement Settings")]
    [SerializeField] float rollTorque = 150f; // Moment obrotowy osi X
    [SerializeField] float pitchTorque = 200f; // Moment obrotowy osi Y
    [SerializeField] float yawTorque = 200f; // Moment obrotowy osi Z
    [SerializeField] float thrust = 75f; // Sila napedu do przodu
    [SerializeField] float upThrust = 75f; // Sila napedu do gory
    [SerializeField] float strafeThrust = 75f; // Sila napedu w bok

    // Ustawienia boostu
    [Header("Boost Settings")]
    [SerializeField] float maxBoostAmount = 20f; // Maksymalna wartosc boostu
    [SerializeField] float boostDeprecationRate = 0.1f; // Tempo utraty boostu
    [SerializeField] float boostRechargeRate = 0.15f; // Tempo odnawiania boostu
    [SerializeField] float boostMultiplier = 10f; // Mnoznik boostu
    bool boosting = false; // Zmienna do sprawdzania czy jest aktywowany boost
    float currentBoostAmount; // Obecna ilosc boostu

    // Ustawienia wytracania prêdkoœci
    [SerializeField, Range(0.001f, 0.999f)]
    float thrustGlideReduction = 0.333f; // Wytracanie predkosci przod
    [SerializeField, Range(0.001f, 0.999f)]
    float upDownGlideReduction = 0.111f; // Wytracanie predkosci gora
    [SerializeField, Range(0.001f, 0.999f)]
    float leftRightGlideReduction = 0.111f; // Wytracanie predkosci bok
    float glide, verticalGlide, horizontalGlide = 0f; // Zmienne do przechowywania wartosci wytracania predkosci

    private Rigidbody rb;
    float thrust1D; // Przechowanie wartosci wejscia od gracza dla napedu statku przod/tyl
    float upDown1D; // Przechowanie wartosci wejscia od gracza dla ruchu statku gora/dol
    float strafe1D; // Przechowanie wartosci wejscia od gracza dla ruchu w boki
    float roll1D; // Przechowanie wartosci wejscia od gracza dla obrotu statku wokol wlasnej osi
    Vector2 pitchYaw; // Wektor do przechowywania wartosci obrotu wokol osi Yaw (Z) i Pitch (Y)

    private CustomQuaternion rotationQuaternion = new CustomQuaternion(0, 0, 0, 1);

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Inicjalizacja komponentu Rigidbody
        currentBoostAmount = maxBoostAmount; // Ustawienie maksymalnej iloœci boostu na starcie
    }

    void FixedUpdate()
    {
        HandleBoosting();
        HandleMove();
    }
    // Metoda do obslugi boostu
    void HandleBoosting()
    {
        if (boosting && currentBoostAmount > 0f)
        {
            currentBoostAmount -= boostDeprecationRate; // Zmniejszenie ilosci boostu
            if (currentBoostAmount <= 0f)
            {
                boosting = false; // Wylaczenie boostu po wyczerpaniu ilosci
            }
        }
        else
        {
            if (currentBoostAmount < maxBoostAmount)
            {
                currentBoostAmount += boostRechargeRate; //tu dawac paczki z energia
            }
        }
    }
    // Metoda do aktualizowania obrotow statku
    void UpdateRotation()
    {
        // Zapisanie wejscia osi obrotu gracza
        float pitchInput = -pitchYaw.y;
        float yawInput = pitchYaw.x;
        float rollInput = roll1D;

        // U¿yj metody Rotate z klasy CustomQuaternion
        CustomQuaternion pitchQuaternion = CustomQuaternion.FromAxisAngle(Vector3.right, pitchInput * 100f * Time.deltaTime);
        CustomQuaternion yawQuaternion = CustomQuaternion.FromAxisAngle(Vector3.up, yawInput * 100f * Time.deltaTime);
        CustomQuaternion rollQuaternion = CustomQuaternion.FromAxisAngle(Vector3.back, rollInput * 100f * Time.deltaTime);
        // Mnozenie 
        rotationQuaternion = CustomQuaternion.Multiply(CustomQuaternion.Multiply(yawQuaternion, pitchQuaternion), rollQuaternion);
        // Normalizacja
        rotationQuaternion.Normalize();
    }
    // Metoda do obslugi ruchu statku
    void HandleMove()
    {
        // Aktualizacja kwaternionu rotacji na podstawie ruchu gracza
        UpdateRotation();

        // Zastosowanie sil i momentow obrotowych przy uzyciu macierzy rotacji
        rb.AddRelativeTorque(rotationQuaternion.Back() * roll1D * rollTorque * Time.deltaTime);
        rb.AddRelativeTorque(rotationQuaternion.Right() * Mathf.Clamp(-pitchYaw.y, -1, 1f) * pitchTorque * Time.deltaTime);
        rb.AddRelativeTorque(rotationQuaternion.Up() * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);

        // Obsluga ruchu statku w trzech wymiarach (przod, gora/dol, bok)
        if (thrust1D > 0.1f || thrust1D < -0.1)
        {
            // Okreœlenie aktualnej sily napedu
            float currentThrust;
            if (boosting)
            {
                currentThrust = thrust * boostMultiplier; // Jesli uzywamy boost to zwiekszamy sile napedu o mnoznik boosta
            }
            else
            {
                currentThrust = thrust;
            }
            // Zastosowanie sily napedu do przodu w kierunku okreslonym przez rotacje statku
            rb.AddRelativeForce(rotationQuaternion.RotateVector(Vector3.forward) * thrust1D * currentThrust * Time.deltaTime);
            // Aktaulizacja wartosci dla pozniejszego wytracania predkosci
            glide = thrust;
        }
        else
        {
            // Wytracanie predkosci, gdy gracz nie rusza sie
            rb.AddRelativeForce(rotationQuaternion.RotateVector(Vector3.forward) * glide * Time.deltaTime);
            glide *= thrustGlideReduction;
        }

        // Obsluga ruchu w gore/dol
        if (upDown1D > 0.1f || upDown1D < -0.1)
        {
            // Zastosowanie sily napedu w gore/dol w zaleznosci od rotacji 
            rb.AddRelativeForce(rotationQuaternion.RotateVector(Vector3.up) * upDown1D * upThrust * Time.fixedDeltaTime);
            // Aktaulizacja wartosci dla pozniejszego wytracania predkosci
            verticalGlide = upDown1D * upThrust;
        }
        else
        {
            // Wytracanie predkosci w gore/dol gdy gracz nie rusza sie
            rb.AddRelativeForce(rotationQuaternion.RotateVector(Vector3.up) * verticalGlide * Time.fixedDeltaTime);
            verticalGlide *= upDownGlideReduction;
        }

        // Obsluga ruchu w bok
        if (strafe1D > 0.1f || strafe1D < -0.1)
        {
            // Zastosowanie sily napedu w bok w zaleznosci od rotacji
            rb.AddRelativeForce(rotationQuaternion.RotateVector(Vector3.right) * strafe1D * strafeThrust * Time.fixedDeltaTime);
            // Aktaulizacja wartosci dla pozniejszego wytracania predkosci
            horizontalGlide = strafe1D * strafeThrust;
        }
        else
        {
            // Wytracanie predkosci w bok gdy gracz nie rusza sie
            rb.AddRelativeForce(rotationQuaternion.RotateVector(Vector3.right) * horizontalGlide * Time.fixedDeltaTime);
            horizontalGlide *= leftRightGlideReduction;
        }
    }


    // Metody obslugjace input 
    public void OnThrust(InputAction.CallbackContext context)
    {
        thrust1D = context.ReadValue<float>(); // naped do przodu
    }

    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe1D = context.ReadValue<float>(); // naped boczny
    }

    public void OnUpDown(InputAction.CallbackContext context)
    {
        upDown1D = context.ReadValue<float>(); //naped gora dol
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        roll1D = context.ReadValue<float>(); // obroty
    }

    public void OnPitchView(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>(); // ruch mysza
        pitchYaw = Vector2.ClampMagnitude(pitchYaw, 1.0f);
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed; // boost
    }


}

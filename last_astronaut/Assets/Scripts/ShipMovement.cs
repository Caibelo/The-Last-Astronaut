using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ShipMovement : MonoBehaviour
{
    [Header("=== Ship Movement Settings ===")]
    [SerializeField]
    private float yawtorque = 500f;
    [SerializeField]
    private float pitchtorque = 1000f;
    [SerializeField]
    private float rolltorque = 1000f;
    [SerializeField]
    private float thrust = 100f;
    [SerializeField]
    private float upthrust = 50f;
    [SerializeField]
    private float strafethrust = 50f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float thrustglidereduction = 0.999f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float updownglidereduction = 0.111f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float leftrightglidereduction = 0.111f;
    float glide, verticalglide, horizontalglide = 0f;
    Rigidbody rb;

    //InputValues
    private float thrust1D;
    private float upDown1D;
    private float strafe1D;
    private float roll1D;
    private Vector2 pitchYaw;

    [SerializeField] private bool isOccupied = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isOccupied) 
        {
          HandleMovement();
        }

    }

    void HandleMovement()
    {
        //Roll --> matematica ------> avvitamento -----> :) <---- io
        rb.AddRelativeTorque(Vector3.back * roll1D * rolltorque * Time.deltaTime);
        //Ptich
        rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYaw.y, -1f, 1f) * pitchtorque * Time.deltaTime);
        //Yaw
        rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawtorque * Time.deltaTime);
        //Trust
        if (thrust1D > 0.1f || thrust1D < -0.1f) 
        { 
          float currentThrust = thrust;

          rb.AddRelativeForce(Vector3.forward * thrust1D * currentThrust * Time.deltaTime);
          glide = thrust;
        }
        else
        { 
          rb.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
          glide *= thrustglidereduction;
        }

        // UP/DOWN
        if (upDown1D > 0.1f || upDown1D < -0.1f)
        {
            float currentThrust = thrust;

            rb.AddRelativeForce(Vector3.up * upDown1D * upthrust * Time.fixedDeltaTime);
            verticalglide = upDown1D * upthrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * verticalglide * Time.fixedDeltaTime);
            verticalglide *= updownglidereduction;
        }

        // Strafing
        if (strafe1D > 0.1f || strafe1D < -0.1f)
        {
            float currentThrust = thrust;

            rb.AddRelativeForce(Vector3.right * strafe1D * upthrust * Time.fixedDeltaTime);
            horizontalglide = strafe1D * strafethrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.right * horizontalglide * Time.fixedDeltaTime);
            horizontalglide *= leftrightglidereduction;
        }
    }

    #region InputMethods

    public void OnThrust(InputAction.CallbackContext context)
    { 
       thrust1D = context.ReadValue<float>();
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

    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
    }

    #endregion

}
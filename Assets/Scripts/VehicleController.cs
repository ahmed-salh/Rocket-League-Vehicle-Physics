using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VehicleController : MonoBehaviour
{
    
    private float steerInput;

    public Suspension[] springBase;

    [Header("Specs")]
    public float wheelBase;
    public float rearTrack;
    public float turnRadius;
    public float maximumVelocity;
    public Transform COG;

    private Rigidbody carRigidBody;
    private Vector3 carVelocity;
    private Vector3 MaxVelocity;
    private float ackermanAngeleLeft;
    private float ackermanAngeleRight;

    private void Awake() {

        carRigidBody = GetComponent<Rigidbody>();
        carRigidBody.centerOfMass = COG.localPosition;
        
    }

    private void Update() {


        steerInput = Input.GetAxis("Horizontal");

        if (steerInput > 0)
        {
            // turning right
            ackermanAngeleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
            ackermanAngeleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;

        }

        else if (steerInput < 0)
        {
            // turning left
            ackermanAngeleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
            ackermanAngeleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;

        } else {

            ackermanAngeleLeft = 0;
            ackermanAngeleRight = 0;
        }

        foreach (Suspension wheel in springBase) {

            if (wheel.wheelFrontLeft)
                wheel.steerAngle = ackermanAngeleLeft;

            if (wheel.wheelFrontRight)
                wheel.steerAngle = ackermanAngeleRight;
        }

    }

    private void FixedUpdate() {

        carVelocity = carRigidBody.velocity;

        // Set the maximum velocity for the car.
        if (carVelocity.sqrMagnitude >= Mathf.Pow(maximumVelocity, 2)) {

            MaxVelocity = carVelocity.normalized * maximumVelocity;
            carRigidBody.velocity = MaxVelocity;
        }
        
    }
}

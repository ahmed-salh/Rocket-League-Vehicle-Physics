using UnityEngine;

public class Suspension : MonoBehaviour
{

    #region Fields
    public bool wheelFrontLeft;
    public bool wheelFrontRight;
    public bool wheelReerLeft;
    public bool wheelReerRight;

    private Rigidbody carRigidBody;
    private float verticalInput;

    [Header("Suspension")]
    public float restLength;
    public float springTravel;
    public float springStiffness;
    public float damperStiffness;

    [Header("Friction")]
    public float brakesFriction;
    public float sideFriction;
    private int brakeInput;

    private float maxLength;
    private float minLength;
    private float lastLength;
    private float springLength;
    private float springVelocity;
   
    private float LongitudeForce;
    private float LateralForce;
    private Vector3 suspensionForce;
    private Vector3 wheelVelocityLS;
    private Vector3 localCarVelocity;

    private float wheelAngle;
    
    [Header("Wheel")]
    public Transform wheelRender;
    public float wheelRadius;
    public float steerAngle;
    public float steerTime;
    public float motorForce;
    public float sidewayForce;
    private float brakeForce;
    #endregion

    private void Awake() {

        carRigidBody = transform.root.GetComponent<Rigidbody>();
        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;
    }

    private void Update() {

        verticalInput = Input.GetAxis("Vertical");
        
        if (Input.GetKey("space")) {

            brakeInput = 1;
           
        } else {
            
            brakeInput = 0;
        }

        // Steer
        wheelAngle = Mathf.Lerp(wheelAngle, steerAngle, steerTime * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.up * wheelAngle);
    }

    private void FixedUpdate() {

        localCarVelocity = transform.InverseTransformDirection(carRigidBody.velocity);

        if (Physics.SphereCast(transform.position, wheelRadius, -transform.up, out RaycastHit hit, maxLength + wheelRadius)) {

            SuspensionForce(hit);
            Brakes();
            ForwardFriction(hit);
            //RollingResistance(hit);
            //Drag();
            LateralFriction(hit);

            RotateWheelRender();
        }
    }

    #region Custom Methods
    private void ForwardFriction(RaycastHit hit) {

        LongitudeForce = verticalInput * motorForce * (1 - brakeInput) - brakeForce;
        carRigidBody.AddForceAtPosition(LongitudeForce * transform.forward, hit.point);

    }

    private void RollingResistance(RaycastHit hit) { 
    
    }

    private void LateralFriction(RaycastHit hit) {

        wheelVelocityLS = transform.InverseTransformDirection(carRigidBody.GetPointVelocity(hit.point));
        LateralForce = wheelVelocityLS.x * sidewayForce;
        carRigidBody.AddForceAtPosition(LateralForce * -transform.right, hit.point);
    }

    private void SuspensionForce(RaycastHit hit) {

        lastLength = springLength;
        //springLength = hit.distance - wheelRadius;
        springLength = hit.distance;
        springLength = Mathf.Clamp(springLength, minLength, maxLength);
        springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;
        suspensionForce = (springStiffness * (restLength - springLength) + damperStiffness * springVelocity) * transform.up;
        carRigidBody.AddForceAtPosition(suspensionForce, hit.point);
    }

    private void Drag() { 
    
    }

    private void Brakes() {

        brakeForce = 0.25f * carRigidBody.mass * (brakesFriction * localCarVelocity.z / Time.fixedDeltaTime) * brakeInput;
    }

    private void RotateWheelRender() {

        wheelRender.Rotate(Vector3.right * Mathf.Rad2Deg * (wheelVelocityLS.z / wheelRadius) * Time.deltaTime * (1 - brakeInput));
        wheelRender.position = transform.position - transform.up * springLength;
    }
    #endregion
}




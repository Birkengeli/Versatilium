using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Character : MonoBehaviour
{
    [System.Flags]
    public enum StatusEffect
    {
        None = 0 << 0,
        FreezeCamera_Tick = 1 << 0,
        FreezeMovement_Tick = 1 << 1,
        DisableCamera_Tick = 1 << 2 | 1 << 0,

        DrivingMode_Tick = 1 << 0 | 1 << 1 | 1 << 2,
    }

    public StatusEffect StatusEffects = StatusEffect.None;

    [Header("Movement Attributes")]
    public float speed = 5;
    public float friction = 10;
    public float sprintSpeedModifier = 2;
    public float jumpHeight = 2;
    public bool hasAirControl;
    Vector3 velocity;
    private float characterHeight;

    [Header("Vertical Attributes")]

    public float gravity = 9.81f;
    public float terminalVelocity = 80.55f;
    public string fallingInformatiom = "[Read Only]";

    [Header("Camera Attributes")]
    public float turnSpeed = 2;
    private Vector2 cameraEuler;

    [Header("Settings")]
    public KeyCode FreeCamera = KeyCode.F1;

    Camera camera;

    void Start()
    {
        //Application.targetFrameRate = 60;

        camera = GetComponentInChildren<Camera>();

        Controller_Spectator.LockCursor(true);

        CapsuleCollider collider = GetComponent<CapsuleCollider>();

        characterHeight = Mathf.Max(collider.height, collider.radius * 1);
    }

    // Update is called once per frame
    void Update()
    {
        float timeStep = Time.deltaTime;

        ControllGravity(timeStep);

        if (!Cursor.visible)
        {
            ControllMovement(timeStep);
            ControllCamera(timeStep);

            transform.position += velocity * timeStep;
        }

        if (Input.GetKeyDown(FreeCamera))
            Controller_Spectator.LockCursor(false, true);



        StatusEffects = 0;
    }

    void CompareStatus(StatusEffect mask, StatusEffect status)
    {


    }

    void ControllCamera(float timeStep)
    {
        bool freezeCamera = (int)StatusEffects >> 0 != 0;

        camera.enabled = !((int)StatusEffects >> 2 != 0);

        if (freezeCamera)
            return;

        float mouseX = Input.GetAxis("Mouse X") * turnSpeed;
        float mouseY = -Input.GetAxis("Mouse Y") * turnSpeed;

        cameraEuler += new Vector2(mouseY, mouseX);
        cameraEuler.x = Mathf.Clamp(cameraEuler.x, -90, 90);

        camera.transform.localEulerAngles = Vector3.right * cameraEuler.x;
        transform.localEulerAngles = Vector3.up * cameraEuler.y;
    }
    void ControllMovement(float timeStep)
    {
        bool freezeMovement = (int)StatusEffects >> 1 != 0;

        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit, characterHeight / 2);

        Vector3 groundNormal = hit.transform != null ? hit.normal : Vector3.up;

        float forward = (Input.GetKey(KeyCode.W) ? 1 : 0) + (Input.GetKey(KeyCode.S) ? -1 : 0);
        float sideways = (Input.GetKey(KeyCode.D) ? 1 : 0) + (Input.GetKey(KeyCode.A) ? -1 : 0);
        float speedModifier = Input.GetKey(KeyCode.LeftShift) ? sprintSpeedModifier : 1;


        Vector3 groundForward = Vector3.Cross(transform.right, groundNormal);
        Vector3 groundRight = Vector3.Cross(groundNormal, groundForward);

        Vector3 moveDirection = (groundForward * forward + groundRight * sideways).normalized;

        Vector3 verticalVelocity = Vector3.Project(velocity, Vector3.down);
        Vector3 horizontalVelocity = velocity - verticalVelocity;

        if (hasAirControl || fallingInformatiom == "I am grounded.")
        {
            velocity -= horizontalVelocity * friction * timeStep;

            if (!freezeMovement)
                velocity += moveDirection * speed * speedModifier * friction * timeStep;
        }
    }

    void ControllGravity(float timeStep)
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit, characterHeight / 2 + 0.05f);

        Vector3 verticalVelocity = Vector3.Project(velocity, Vector3.down);

        bool isGrounded = hit.transform != null && velocity.y < 0.1f;
        float gravityStep = gravity * timeStep;

        if (isGrounded)
        {
            velocity -= verticalVelocity;
            transform.position += Vector3.up * ((characterHeight / 2) - hit.distance); // Doesn't work well with a moving car

            if (false && hit.transform.tag == "Vehicle")
            {
                Rigidbody CarRB = hit.transform.GetComponent<Rigidbody>();

                Vector3 velDifference = CarRB.velocity - velocity;

                transform.position += CarRB.velocity * Time.fixedDeltaTime;

            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                float jumpStrength = Mathf.Sqrt(-2f * -gravity * jumpHeight); // The true strength of the jump
                velocity += Vector3.up * jumpStrength;
            }
        }
        else
        {
            //velocity -= verticalVelocity * gravityStep;
            //velocity += Vector3.down * terminalVelocity * gravityStep;

            velocity -= Vector3.up * gravityStep;
        }

        {
            float currentFallspeed = Mathf.Round(verticalVelocity.magnitude * 100) / 100;


            if (isGrounded)
                fallingInformatiom = "I am grounded.";
            else
                fallingInformatiom = "Fallspeed: " + currentFallspeed + " m/s.";
        }

    }
}
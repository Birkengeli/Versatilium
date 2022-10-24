using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Enemy : MonoBehaviour
{
    public enum EnemyTypes
    {
        Turret, Humanoid
    }
    public bool debugMode = true;

    [Header("Attack")]
    public float DetectionRange = 10;
    public float AimSpeed = 100;
    public float ConeOfFire = 45;
    public bool isLeadingTarget = true;
    public Weapon_Versatilium Weapon;

    [Header("Settings")]
    public EnemyTypes enemyType;
    public bool isInvincible;
    public Transform player;
    public Weapon_Versatilium.Sound[] Sounds;
    public float rememberPlayerFor = 3f;
    private float rememberPlayer_Timer = -1;
    private bool stillRemembersPlayer { get { return rememberPlayer_Timer > 0; } }

    [Header("Turret Behavior")]
    public float ActivationTime = 1f;
    public float ActivationTime_Timer;
    public bool isRetraciting;

    public Vector2 viewEuler;

    [Header("Humanoid Behavior")]
    public float moveSpeed = 2;
    Vector3 previousPosition;
    public bool isInCombat;

    private Vector3 player_LastKnownLocation;


    // Start is called before the first frame update
    void Start()
    {
        if (enemyType == EnemyTypes.Turret)
        {
            transform.GetChild(0).localPosition = -Vector3.forward * 0.9f;
            isInvincible = true;
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float timeStep = Time.deltaTime;

        if (enemyType == EnemyTypes.Turret)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            Transform Turret_Hinge = transform.GetChild(0);
            Transform Turret_Turret = Turret_Hinge.GetChild(0);

            RaycastHit hit;
            Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, DetectionRange + 0.1f);

            bool hasDetectedPlayer = (distanceToPlayer <= DetectionRange && hit.transform != null && hit.transform.tag == "Player");

            if (!hasDetectedPlayer && !stillRemembersPlayer)
            {


                if (ActivationTime_Timer < 0)
                {
                    isInvincible = true;



                }
                else
                {
                    float degreesOffForward = LookAt(Turret_Hinge.position + -transform.up, Turret_Turret.forward, Turret_Hinge, Turret_Turret);
                    if (degreesOffForward < 1)
                    {
                        ActivationTime_Timer -= timeStep;

                        float newZ = (1f - (ActivationTime_Timer / ActivationTime)) * 0.9f;
                        Turret_Hinge.localPosition = -Vector3.forward * newZ;
                    }
                }
            }

            if (hasDetectedPlayer || stillRemembersPlayer)
            {
                if (hasDetectedPlayer)
                {
                    rememberPlayer_Timer = rememberPlayerFor;
                    player_LastKnownLocation = player.position;
                }

   

                if (ActivationTime_Timer > ActivationTime)
                {
                    isInvincible = false;


                    Vector3 targetPostion = hasDetectedPlayer ? player.position : player_LastKnownLocation;

                    if(isLeadingTarget && hasDetectedPlayer)
                        targetPostion = Target_LeadShot(player.position, Weapon.WeaponStats.Projectile_Speed);

                    float degreesOff = (1f - LookAt(targetPostion, Turret_Turret.forward, Turret_Hinge, Turret_Turret)) * 180;
                    bool fire = false;

                    if(degreesOff > ConeOfFire)
                        fire = true;

                    Weapon.OnFire(fire ? Weapon_Versatilium.TriggerTypes.SemiAutomatic : Weapon_Versatilium.TriggerTypes.None);

                    

                }
                else
                {
                    ActivationTime_Timer += timeStep;

                    float newZ = (1f - (ActivationTime_Timer / ActivationTime)) * 0.9f;

                    Turret_Hinge.localPosition = -Vector3.forward * newZ;

                }
            }
            rememberPlayer_Timer -= Time.deltaTime;
        }
        
        if (enemyType == EnemyTypes.Humanoid)
        {
            Vector3 velocity = transform.position - previousPosition;
            Vector3 direction = velocity.normalized;
            float distance = velocity.magnitude;
            previousPosition = transform.position;

            if (debugMode)
                Debug.DrawRay(transform.position, direction * 100, Color.red);

            #region Animation

            Animator anim = GetComponentInChildren<Animator>();

            anim.SetFloat("Forward", (distance / timeStep) / moveSpeed);
												#endregion

												#region Movement
												float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < DetectionRange || isInCombat)
            {
                if (!isInCombat)
                {
                    isInCombat = true;
                    anim.SetTrigger("onCombat");
                }

                transform.LookAt(player);
                Weapon.OnFire(Weapon_Versatilium.TriggerTypes.SemiAutomatic);

                // Where can I go?
                float distanceToWall_Right = 0;
                float distanceToWall_Left = 0;

                for (int i = 0; i < 2; i++)
                {
                    RaycastHit hit;
                    Physics.Raycast(transform.position, transform.right * (i == 0 ? 1 : -1), out hit);

                    if (hit.transform != null)
                    {
                        if(i == 0)
                            distanceToWall_Right = hit.distance;
                        else
                            distanceToWall_Left = hit.distance;
                    }

                }

                
                    transform.position += (transform.forward + transform.right * (distanceToWall_Right > 1 ? 1 : 0)).normalized * moveSpeed * timeStep;
            }
												#endregion


												{ // End Stuff

																
            }
        }
    }


    float LookAt(Vector3 targetPosition, Vector3 currentForward, Transform horizontalTransform, Transform verticalTransform)
    {
        /// I think it is possible to get the vertical need from a static point, so it doens't turn awkwardly and excessibvely while rotating

        Vector3 directionToTarget = targetPosition - horizontalTransform.position;

        float dot_Forward = Vector3.Dot(directionToTarget.normalized, -verticalTransform.up);
        float dot_Vertical = Vector3.Dot(verticalTransform.forward, directionToTarget.normalized);
        float dot_Horizontal = Vector3.Dot(horizontalTransform.right, directionToTarget.normalized);





        if (true)
        {
            bool isCloseEnough = dot_Forward > 0.999f;
            bool isToMyRight = dot_Horizontal < 0;
            bool isAboveMe = dot_Vertical < 0;

            float lerpKickIn = 1f * 0.01f;
            float lerpPercentage = dot_Forward > (1f - lerpKickIn) ? (1f - dot_Forward) / lerpKickIn : 1;

            float input_X = Time.deltaTime * AimSpeed * (isAboveMe ? 1 : -1);
            float input_Y = Time.deltaTime * AimSpeed * (isToMyRight ? 1 : -1);

            viewEuler += new Vector2(input_X, -input_Y) * lerpPercentage;
            viewEuler.x = Mathf.Clamp(viewEuler.x, -50, 50);

            verticalTransform.localEulerAngles = Vector3.right * viewEuler.x;
            horizontalTransform.localEulerAngles = Vector3.forward * viewEuler.y;
        }

        if (false && dot_Forward < 0.999f)
        {
            dot_Horizontal = Mathf.Round(dot_Horizontal * 100) / 100;

            bool isCloseEnough = dot_Forward > 0.999f;
            bool isToMyRight = dot_Horizontal <= 0;
            bool isAboveMe = dot_Vertical < 0;

            float horizontalDegrees = 180f - (1f - dot_Horizontal) * 180;

            print(Mathf.Round(horizontalDegrees) + " Degrees");

            float degreesASecond = AimSpeed * Time.deltaTime;
            float degreesOff = Mathf.Abs(horizontalDegrees);

            float input_X = Time.deltaTime * AimSpeed * (isAboveMe ? 1 : -1);
            float input_Y = Mathf.Min(degreesASecond, degreesOff) * (isToMyRight ? 1 : -1);

            input_X = 0; // Debug

            viewEuler += new Vector2(input_X, -input_Y);
            viewEuler.x = Mathf.Clamp(viewEuler.x, -50, 50);

            verticalTransform.localEulerAngles = Vector3.right * viewEuler.x;
            horizontalTransform.localEulerAngles = Vector3.forward * viewEuler.y;
        }

        return (1f - dot_Forward) * 180;
    }

    Vector3 targetPosition_Previous;

    Vector3 Target_LeadShot(Vector3 targetPosition_Current, float projectileSpeed) // I believe my inconsistent framerate causes some leading issues.
    {
        float timeStep = Time.deltaTime;

        Vector3 targetVelocity = targetPosition_Current - targetPosition_Previous;
        float targetSpeed = targetVelocity.magnitude / timeStep;
        float targetDistance = Vector3.Distance(targetPosition_Current, transform.position);

        float timeToCrossDistance = targetDistance / projectileSpeed;
        float oneTickAhead = (1 + timeStep);
        Vector3 targetPosition_Lead = targetPosition_Current + targetVelocity.normalized * targetSpeed * timeToCrossDistance;

        if (debugMode)
        {
            float crossSize = 1f / 2f;

            Vector3 debugPosition = targetPosition_Lead;

            Debug.DrawRay(debugPosition - Vector3.forward * crossSize, Vector3.forward * crossSize * 2, Color.red);
            Debug.DrawRay(debugPosition - Vector3.right * crossSize, Vector3.right * crossSize * 2, Color.blue);
            Debug.DrawRay(debugPosition - Vector3.up * crossSize, Vector3.up * crossSize * 2, Color.green);
        }

        targetPosition_Previous = targetPosition_Current;

        return targetPosition_Lead;

    }
    
}

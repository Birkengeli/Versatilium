using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Enemy : MonoBehaviour
{
    public enum EnemyTypes
    {
        Turret,
    }
    public bool debugMode = true;

    [Header("Attack")]
    public float DetectionRange = 10;
    public float AimSpeed = 100;
    public Weapon_Versatilium.WeaponStatistics Weapon;

    [Header("Defense")]
    public int HealthMax = 100;

    [Header("Settings")]
    public EnemyTypes enemyType;
    public bool isInvincible;
    public Transform player;

    [Header("Turret Behavior")]
    public float ActivationTime = 1f;
    private float ActivationTime_Timer;

    public Vector2 viewEuler;

    // Start is called before the first frame update
    void Start()
    {
        
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

            Vector3 targetLocation = player.position;

            if (distanceToPlayer > DetectionRange)
            {

 

                if (ActivationTime_Timer < 0)
                {
                    isInvincible = true;



                }
                else
                {
                    float forwardDot = LookAt(Turret_Hinge.position + -transform.up, Turret_Turret.forward, Turret_Hinge, Turret_Turret);

                    if (forwardDot > 0.90)
                    {
                        ActivationTime_Timer -= timeStep;

                        float newZ = (1f - (ActivationTime_Timer / ActivationTime)) * 0.9f;

                        Turret_Hinge.localPosition = -Vector3.forward * newZ;
                    }
                }
            }

            if (distanceToPlayer <= DetectionRange)
            {
                if (ActivationTime_Timer > ActivationTime)
                {
                    isInvincible = false;

                    LookAt(player.position, Turret_Turret.forward, Turret_Hinge, Turret_Turret);

                }
                else
                {
                    ActivationTime_Timer += timeStep;

                    float newZ = (1f - (ActivationTime_Timer / ActivationTime)) * 0.9f;

                    Turret_Hinge.localPosition = -Vector3.forward * newZ;

                }
            }
        }
        
    }


    float LookAt(Vector3 targetPosition, Vector3 currentForward, Transform horizontalTransform, Transform verticalTransform)
    {
        Vector3 directionToTarget = targetPosition - horizontalTransform.position;

        float dot_Forward = Vector3.Dot(directionToTarget.normalized, -verticalTransform.up);
        float dot_Vertical = Vector3.Dot(verticalTransform.forward, directionToTarget.normalized);
        float dot_Horizontal = Vector3.Dot(horizontalTransform.right, directionToTarget.normalized);

        bool isCloseEnough = dot_Forward > 0.999f;
        bool isToMyRight = dot_Horizontal < 0;
        bool isAboveMe = dot_Vertical < 0;

        if (true)
        {
            float input_X = Time.deltaTime * AimSpeed * (isAboveMe ? 1 : -1);
            float input_Y = Time.deltaTime * AimSpeed * (isToMyRight ? 1 : -1);

            viewEuler += new Vector2(input_X, -input_Y);
            viewEuler.x = Mathf.Clamp(viewEuler.x, -50, 50);

            verticalTransform.localEulerAngles = Vector3.right * viewEuler.x;
            horizontalTransform.localEulerAngles = Vector3.forward * viewEuler.y;
        }

        return dot_Forward;
    }

    
}

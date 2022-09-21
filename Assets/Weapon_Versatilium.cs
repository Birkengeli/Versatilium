using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Versatilium : MonoBehaviour
{

  

   
    public bool debugMode = true;

    [Header("Weapon Settings")]
    public Attack_Mode_Settings currentMode;
    public Attack_Projectile_Setting currentProjectile;
    public Attack_Delivery_Setting currentDelivery;
    public Attack_OnHit_Settings currentOnHit;

    [Header("Misc. Settings")]
    public Transform playerEyes;

    // Start is called before the first frame update
    void Start()
    {
        if (playerEyes == null)
            playerEyes = GetComponentInChildren<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            OnFire(currentMode);
    }

				#region OnFire
				[System.Serializable]
    public class Attack_Mode_Settings
    {
        [Header("General")]
        public float fireRate = 2;

        [Header("Charging")]
        public bool basedOffCharge;
        public float Charge_minimumTime = 0.1f;
        public float Charge_maximumTime = 1f;

        [Header("Other")]
        public bool doubleTab;
    }


    void OnFire(Attack_Mode_Settings attackMode)
    {

        CreateProjectile(currentProjectile);
        
    }

				#endregion

				#region Projectile

				[System.Serializable]
    public class Attack_Projectile_Setting
    {
        [Header("General")]
        public int Projectile_Count = 1;
        public float Projectile_Deviation = 0;
        public bool Projectile_FixedSpread;

        [Header("Physics")]
        public bool UseProjectiles;
        public float Projectile_Physics_Speed = 25f; // TF2's Grenade Launcher moves at 23m/s
        public float Projectile__Physics_Gravity = 9.807f;
    }


    public Vector2 TestDeviation;

    void CreateProjectile(Attack_Projectile_Setting projectileMode)
    {

        if (!projectileMode.UseProjectiles) // Hitscan
        {

            for (int i = 0; i < projectileMode.Projectile_Count; i++)
            {
                float projectile_Deviation = projectileMode.Projectile_Deviation;

                Vector3 rayDeviation = playerEyes.right * Random.Range(-projectile_Deviation, projectile_Deviation) + playerEyes.up * Random.Range(-projectile_Deviation, projectile_Deviation);

                Vector3 rayOrigin = playerEyes.position;
                Vector3 rayDirection = playerEyes.forward + rayDeviation;

                RaycastHit hit;
                Physics.Raycast(rayOrigin, rayDirection, out hit);

                if(debugMode)
																{
                    float debugRange = hit.distance + (hit.distance == 0 ? 100 : 0);

                    Debug.DrawRay(rayOrigin, rayDirection * debugRange, Color.red, currentMode.fireRate);
																}

                /// Raycast hit gives me a lot of information I need to take with me
                /// hit.point being the biggest one.
                /// 

                DetectTargets(currentDelivery, hit.point);

            }
        }
    }
				#endregion

				#region Delivery

				[System.Serializable]
    public class Attack_Delivery_Setting
    {
        [Header("General")]
        public bool deleteProjectileOnImpact = true;
        public int bounceCount = 0;
        public bool freezeOnImpact = true;

        [Header("Triggering OnHit")]
        public bool instant = true;
        public float lingerTimeAtLocation = 0;
    }

    void DetectTargets(Attack_Delivery_Setting deliveryMode, Vector3 impactPosition)
    {


        Collider[] hits = Physics.OverlapSphere(impactPosition, 0.01f);

        for (int i = 0; i < hits.Length; i++)
        {
            Controller_Enemy enemyScript = hits[i].transform.GetComponent<Controller_Enemy>();

            if (enemyScript != null)
            {
                OnHit(currentOnHit, enemyScript);
            }


            
        }


        OnHit(currentOnHit, null);


    }

				#endregion

				#region OnHit

				[System.Serializable]
    public class Attack_OnHit_Settings
    {
        [Header("General")]
        public float damage;
        public float knockback;

        public void OnHit(string testValue)
        {
            Debug.Log(testValue + " + " + damage + " actual damage.");

        }
    }

    void OnHit(Attack_OnHit_Settings onHitMode, Controller_Enemy enemyScript)
				{

        if (enemyScript != null) // Hit enemy
        {
            // Damage

            // Knockback
            

            Debug.Log("PEW! ~Splunk~");


        }

        if (enemyScript == null) // Hit terrain
        {
            Debug.Log("PEW! ~Thud~");
        }
    }

				#endregion

}

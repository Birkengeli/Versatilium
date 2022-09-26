using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Versatilium : MonoBehaviour
{

    public enum ProjectileTypes {Hitscan, Projectile }

    public bool debugMode = true;

    [Header("General")]
    public float damage = 10;
    public float knockback = 10;
    public float fireRate = 2;
    float fireRate_CD;
    public int PelletCount = 1;
    public float Deviation = 0.01f;

    [Header("Alt-Fire: Charge")]
    public bool trigger_scaleOffCharge = false;
    public Weapon_Versatilium altFire_Charge;
    public float Charge_minimumTime = 0.1f;
    public float Charge_maximumTime = 1f;
    float Charge_current;

    [Header("Alt-Fire: Double Tap")]
    public bool trigger_doubleTab;
    public Weapon_Versatilium altFire_doubleTap;


    [Header("Physics")]
    public ProjectileTypes ProjectileType = ProjectileTypes.Hitscan;
    public float Projectile_Speed = 25f; // TF2's Grenade Launcher moves at 23m/s
    public float Projectile_Gravity = 9.807f;
    public bool deleteProjectileOnImpact = true;
    public int bounceCount = 0;
    public bool freezeOnImpact = true;

    [Header("Misc. Settings")]
    public float lingerTimeAtLocation = 0;

    [Header("Variables")]
    public Transform playerEyes;
    public Transform Model_Weapon;
    public Transform Origin_Barrel;
    public ParticleSystem gunParticles;

    public void OnHit(string testValue)
    {
        Debug.Log(testValue + " + " + damage + " actual damage.");

    }

    // Start is called before the first frame update
    void Start()
    {
        if (playerEyes == null)
            playerEyes = GetComponentInChildren<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        
       OnFire();
    }

				#region OnFire


    void OnFire()
    {
        fireRate_CD -= Time.deltaTime;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (fireRate_CD < 0)
            {
                fireRate_CD = 1f / fireRate;
                CreateProjectile();
            }
        }
        
    }

				#endregion

				#region Projectile

    void CreateProjectile()
    {

        if (ProjectileType == ProjectileTypes.Hitscan)
        {

            for (int i = 0; i < PelletCount; i++)
            {
                Vector3 rayDeviation = playerEyes.right * Random.Range(-Deviation, Deviation) + playerEyes.up * Random.Range(-Deviation, Deviation);

                Vector3 rayOrigin = playerEyes.position;
                Vector3 rayDirection = playerEyes.forward + rayDeviation;

                RaycastHit hit;
                Physics.Raycast(rayOrigin, rayDirection, out hit);

                if(debugMode)
																{
                    float debugRange = hit.distance + (hit.distance == 0 ? 100 : 0);

                    Debug.DrawRay(rayOrigin, rayDirection * debugRange, Color.red, fireRate_CD); // The firerate CD is 1f/ firerate in this tic


                    Debug.DrawLine(Origin_Barrel.position, rayOrigin + rayDirection * debugRange, Color.blue, fireRate_CD); // The firerate CD is 1f/ firerate in this tic
                }

                /// Raycast hit gives me a lot of information I need to take with me
                /// hit.point being the biggest one.
                /// 

                gunParticles.Play();

                DetectTargets(hit.point);

            }
        }
    }
				#endregion

				#region Delivery

		
    void DetectTargets(Vector3 impactPosition)
    {


        Collider[] hits = Physics.OverlapSphere(impactPosition, 0.01f);

        for (int i = 0; i < hits.Length; i++)
        {
            Controller_Enemy enemyScript = hits[i].transform.GetComponent<Controller_Enemy>();

            if (enemyScript != null)
            {
                OnHit(enemyScript);
            }


            
        }

    }

				#endregion

				#region OnHit


    void OnHit(Controller_Enemy enemyScript)
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


    void ParticlesAndEffects(int state)// State 0 = OnFire
				{
        if(state == 0) // On Fire
								{

								}
				}

}

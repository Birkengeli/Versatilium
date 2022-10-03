using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Versatilium : MonoBehaviour
{

    public enum ProjectileTypes {Hitscan, ProjectilePhysics, ProjectileSimulated }

    [System.Flags]
    public enum OptionReplace // i think I should actually sort this in Weapon parts.
    {
        Nothing = 0 << 0,

        Damage = 1 << 0,
        Firerate = 1 << 1,
        TriggerPrimary = 1 << 2,
        TriggerSecondary = 1 << 3,
    }

    [System.Serializable]
    public class Sound
    {
        public string name = "No Name";
        public AudioClip[] audioClips;
        [Header("Settings")]
        public float volumeScale = 1;
        public randomTypes random = randomTypes.NeverTwice;

        private int lastIndex = 999;

        public static void Play(string name, Sound[] sounds, AudioSource audioSource, bool ignoreMissing = false)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].name == name)
                {
                    Play(sounds[i], audioSource);
                    return;
                }
            }

            if(!ignoreMissing)
                Debug.LogWarning("Could Not find the Sound called '" + name + "'.");
        }

        public static void Play(Sound sound, AudioSource audioSource)
        {
            AudioClip clip = sound.audioClips[0];

												#region Random Order
												if (sound.random == randomTypes.Random)
                clip = sound.audioClips[Random.Range(0, sound.audioClips.Length)];

            if (sound.random == randomTypes.NeverTwice)
            {
                int randomIndex = -1;
                while (true)
                {
                    randomIndex = Random.Range(0, sound.audioClips.Length);

                    if (randomIndex != sound.lastIndex)
                    {
                        sound.lastIndex = randomIndex;
                        clip = sound.audioClips[randomIndex];

                        break;
                    }
                }
            }

            if (sound.random == randomTypes.Sequential)
            {
                int clipCount = sound.audioClips.Length;

                if (sound.lastIndex >= clipCount)
                    sound.lastIndex = 0;

                clip = sound.audioClips[sound.lastIndex];
                sound.lastIndex++;
            }

												#endregion

												audioSource.PlayOneShot(clip, sound.volumeScale);
        }

        public enum randomTypes
        {Sequential, NeverTwice, Random }
    }

    public Sound[] Sounds;

    [System.Serializable]
    public class WeaponStatistics
				{

        public enum TriggerTypes
        {
            SemiAutomatic, Automatic, Charge
        }

        public OptionReplace WhatIsReplaced;

        [Header("General")]
        public float damage = 10;
        public float knockback = 10;
        public float fireRate = 2;
        public int PelletCount = 1;
        public float Deviation = 0.01f;
        public TriggerTypes triggerType = TriggerTypes.SemiAutomatic;

        [Header("Alt-Fire: Charge")]
        public bool trigger_scaleOffCharge = false;
        public float Charge_minimumTime = 0.1f;
        public float Charge_maximumTime = 1f;
        [HideInInspector] public float Charge_current;

        [Header("Alt-Fire: Double Tap")]
        public bool trigger_doubleTab;

        [Header("Physics")]
        public ProjectileTypes ProjectileType = ProjectileTypes.Hitscan;
        public float Projectile_Speed = 25f; // TF2's Grenade Launcher moves at 23m/s
        public float Projectile_Gravity = 9.807f;
        public bool deleteProjectileOnImpact = true;
        public int bounceCount = 0;
        public bool freezeOnImpact = true;
        public bool inheritUserVelocity = true;

        [Header("Misc. Settings")]
        public float lingerTimeAtLocation = 0;
        public bool Visuals_UseCustom;
        public Tools_Animator.CustomAnimation Visuals_Projectile;
        public float Visuals_ProjectileScale = 1f;
        public bool isWieldedByPlayer = true;

        [HideInInspector]
        public Controller_Character characterController;
    }

    [System.Serializable]
    public struct Projectile
    {
        public Transform visualTransform;
        public Tools_Animator anim;
        public Vector3 velocity;
        public Vector3 position;
        public float gravity;
        public float lifeTime;
    }

    public bool debugMode = true;

    public WeaponStatistics WeaponStats;
    public WeaponStatistics WeaponStats_Alt;

    [Header("Variables")]
    public Transform playerEyes;
    public Transform Model_Weapon;
    public Transform Origin_Barrel;
    public ParticleSystem gunParticles;
    public GameObject Prefab_Projectile;

    public List<Projectile> Projectiles;
    public float fireRate_GlobalCD;

    private AudioSource audioSource;

    public void OnHit(string testValue)
    {
        Debug.Log(testValue + " + " + 99999 + " actual damage.");

    }

    // Start is called before the first frame update
    void Start()
    {
        if (playerEyes == null)
            playerEyes = GetComponentInChildren<Camera>().transform;

        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        float timeStep = Time.deltaTime;
        float projectileMaxLife = 10f;

        OnFire();

        for (int i = 0; i < Projectiles.Count; i++)
        {
            Projectile currentProjectile = Projectiles[i];

            currentProjectile.lifeTime += timeStep;
            currentProjectile.velocity += Vector3.down * currentProjectile.gravity * timeStep;
            currentProjectile.position += currentProjectile.velocity * timeStep;


            if (currentProjectile.visualTransform != null)
            {
                currentProjectile.visualTransform.position = currentProjectile.position;
                currentProjectile.anim.Play("Idle");
            }

           Projectiles[i] = currentProjectile;


            if(debugMode)
												{
                float ColorGradeUp = currentProjectile.lifeTime / projectileMaxLife;
                float ColorGradeDown = 1f - ColorGradeUp;

                Color customColor = new Color(ColorGradeDown, 0, ColorGradeUp);

                Debug.DrawRay(currentProjectile.position, -currentProjectile.velocity * timeStep, customColor, 1f); // Fun Fact: I am drawing this ray backwards (to compensate for already applying velocity)
												}

            if (currentProjectile.lifeTime > projectileMaxLife)
            {
                if (currentProjectile.visualTransform != null)
                    Destroy(currentProjectile.visualTransform.gameObject); // This should actually be pooled.
                Projectiles.RemoveAt(i);
                i--;
            }
        }
    }

    #region OnFire


    void OnFire()
    {
        WeaponStatistics currentStats = WeaponStats;  // The Default Firemode

        fireRate_GlobalCD -= Time.deltaTime;


        if (currentStats.triggerType == WeaponStatistics.TriggerTypes.SemiAutomatic)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && fireRate_GlobalCD < 0) // basic Fire
            {
                Sound.Play("Fire", Sounds, audioSource);

                fireRate_GlobalCD = 1f / currentStats.fireRate;
                CreateProjectile(currentStats);
            }

       }


        if (currentStats.triggerType == WeaponStatistics.TriggerTypes.Automatic)
        {
            if (Input.GetKey(KeyCode.Mouse0) && fireRate_GlobalCD < 0) // basic Fire
            {
                Sound.Play("Fire", Sounds, audioSource);

                fireRate_GlobalCD = 1f / currentStats.fireRate;
                CreateProjectile(currentStats);
            }

        }

        if (currentStats.triggerType == WeaponStatistics.TriggerTypes.Charge)
        {

            if (Input.GetKeyDown(KeyCode.Mouse0) && fireRate_GlobalCD < 0) // Start Charge
                WeaponStats_Alt.Charge_current = 0;
            
            if (Input.GetKey(KeyCode.Mouse0) && fireRate_GlobalCD < 0) // Charging
                WeaponStats_Alt.Charge_current += Time.deltaTime;

            if (Input.GetKeyUp(KeyCode.Mouse0) && fireRate_GlobalCD < 0) // Release Charge
            {
                if (WeaponStats_Alt.Charge_current < WeaponStats_Alt.Charge_minimumTime)
                {
                    // On Tap fire

                    Sound.Play("Fire", Sounds, audioSource);

                    fireRate_GlobalCD = 1f / currentStats.fireRate;
                    CreateProjectile(currentStats);
                }
                else
                {
                    // On Charged shot

                    Sound.Play("Charged Fire", Sounds, audioSource);

                    fireRate_GlobalCD = 1f / WeaponStats_Alt.fireRate;
                    CreateProjectile(WeaponStats_Alt);
                }

                WeaponStats_Alt.Charge_current = 0;
            }

        }

    }

				#endregion

				#region Projectile

    void CreateProjectile(WeaponStatistics currentStats)
    {
        gunParticles.Play();

        #region Hitscan
        if (currentStats.ProjectileType == ProjectileTypes.Hitscan)
        {

            for (int i = 0; i < currentStats.PelletCount; i++)
            {
                Vector3 rayDeviation = playerEyes.right * Random.Range(-currentStats.Deviation, currentStats.Deviation) + playerEyes.up * Random.Range(-currentStats.Deviation, currentStats.Deviation);

                Vector3 rayOrigin = playerEyes.position;
                Vector3 rayDirection = playerEyes.forward + rayDeviation;

                RaycastHit hit;
                Physics.Raycast(rayOrigin, rayDirection, out hit);

                if(debugMode)
																{
                    float debugRange = hit.distance + (hit.distance == 0 ? 100 : 0);

                    Debug.DrawRay(rayOrigin, rayDirection * debugRange, Color.red, fireRate_GlobalCD); // The firerate CD is 1f/ firerate in this tic


                    Debug.DrawLine(Origin_Barrel.position, rayOrigin + rayDirection * debugRange, Color.blue, fireRate_GlobalCD); // The firerate CD is 1f/ firerate in this tic
                }

                /// Raycast hit gives me a lot of information I need to take with me
                /// hit.point being the biggest one.
                /// 

       
                DetectTargets(currentStats, hit.point);

            }


        }
        #endregion

        #region Simulated Projectile

        if (currentStats.ProjectileType == ProjectileTypes.ProjectileSimulated)
        {
            float timeStep = Time.deltaTime;

            for (int i = 0; i < currentStats.PelletCount; i++)
            {
                Vector3 rayDeviation = playerEyes.right * Random.Range(-currentStats.Deviation, currentStats.Deviation) + playerEyes.up * Random.Range(-currentStats.Deviation, currentStats.Deviation);

                Vector3 rayOrigin = playerEyes.position;
                Vector3 rayDirection = playerEyes.forward + rayDeviation;

                RaycastHit hit;
                Physics.Raycast(rayOrigin, rayDirection, out hit);
                Vector3 projectileDirection = hit.transform != null ? ( hit.point - Origin_Barrel.position).normalized : (rayDirection);

                Projectile currentProjectile = new Projectile();

                currentProjectile.gravity = currentStats.Projectile_Gravity;
                currentProjectile.position = Origin_Barrel.position;
                currentProjectile.velocity = projectileDirection * currentStats.Projectile_Speed;

                if(currentStats.inheritUserVelocity && currentStats.isWieldedByPlayer)
																{
                    if (currentStats.characterController == null)
                        currentStats.characterController = transform.GetComponent<Controller_Character>();

                    currentProjectile.velocity += currentStats.characterController.velocity;
                }

                if (currentStats.Visuals_UseCustom)
                {
                    currentProjectile.visualTransform = Instantiate(Prefab_Projectile).transform;
                    currentProjectile.visualTransform.position = currentProjectile.position;
                    currentProjectile.visualTransform.localScale = Vector3.one * currentStats.Visuals_ProjectileScale;

                    currentProjectile.anim = currentProjectile.visualTransform.GetComponent<Tools_Animator>();
                    currentProjectile.anim.Animations[0] = currentStats.Visuals_Projectile;

                    if(true) // This part is what corrects the sprite, but right now I don't want the sprite to start in my face.
                    { 
                        currentProjectile.anim.Play("Idle", true);
                        currentProjectile.anim.OnTick();
                    }
                }

                Projectiles.Add(currentProjectile);

                if (debugMode)
                {

                    Debug.DrawRay(currentProjectile.position, currentProjectile.velocity * timeStep, Color.red, fireRate_GlobalCD); // The firerate CD is 1f/ firerate in this tic
                    Debug.DrawLine(Origin_Barrel.position, rayOrigin + rayDirection * currentStats.Projectile_Speed * timeStep, Color.blue, fireRate_GlobalCD); // The firerate CD is 1f/ firerate in this tic
                }

            }

            
           
        }
        #endregion

    }
    #endregion

    #region Delivery


    void DetectTargets(WeaponStatistics currentStats, Vector3 impactPosition)
    {


        Collider[] hits = Physics.OverlapSphere(impactPosition, 0.01f);

        for (int i = 0; i < hits.Length; i++)
        {
            Controller_Enemy enemyScript = hits[i].transform.GetComponent<Controller_Enemy>();

            if (enemyScript != null)
            {
                OnHit(currentStats, enemyScript);
            }


            
        }

    }

				#endregion

				#region OnHit


    void OnHit(WeaponStatistics currentStats, Controller_Enemy enemyScript)
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Versatilium : MonoBehaviour
{
				#region Enums
				public enum ProjectileTypes
    {
        Hitscan, ProjectilePhysics, ProjectileSimulated
    }

    public enum ChargeOptions // I want damage and pellets at least.
    {
        Nothing, 
    }

    public enum TriggerTypes
    {
        None, SemiAutomatic, Automatic, Charge, Sight
    }

    [System.Flags]
    public enum OptionReplace // i think I should actually sort this in Weapon parts.
    {
        Nothing = 0 << 0,

        Damage = 1 << 0,
        Firerate = 1 << 1,
        TriggerPrimary = 1 << 2,
        TriggerSecondary = 1 << 3,
    }
				#endregion

				#region Classes & Structs
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

            if (sound.audioClips.Length < 2) // If it's set to never twice and it's 1 or zero files, it will never find it.
                sound.random = randomTypes.Random;

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
    public struct Projectile
    {
        public Transform visualTransform;
        public Tools_Animator anim;
        public Vector3 velocity;
        public Vector3 position;
        public float gravity;
        public float lifeTime;

        public WeaponStatistics projectileStats;
    }

    [System.Serializable]
    public class WeaponStatistics
				{
        [Header("General")]
        public TriggerTypes triggerType = TriggerTypes.SemiAutomatic;
        public float damage = 10;
        public float knockback = 10;
        public float fireRate = 2;
        public int PelletCount = 1;
        public float Deviation = 0.01f;

        [Header("Burst (Semi Automatic Only)")]
        public bool firesInBurst = false;
        public int burstCount = 3;
        public int burst_fireRate = 4;
        [HideInInspector] public int burstCounter = 0;


        [Header("Physics")]
        public ProjectileTypes ProjectileType = ProjectileTypes.Hitscan;
        public float Projectile_Speed = 25f; // TF2's Grenade Launcher moves at 23m/s
        public float Projectile_Gravity = 9.807f;
       // public bool deleteProjectileOnImpact = true;
       // public int bounceCount = 0;
      //  public bool freezeOnImpact = true;
        public bool inheritUserVelocity = true;

        [Header("Misc. Settings")]
        //  public float lingerTimeAtLocation = 0;
        public Tools_Animator.CustomAnimation Visuals_Projectile;
        public float Visuals_ProjectileScale = 1f;

 

        [HideInInspector]
        public Controller_Character characterController;
    }



				#endregion

				public bool debugMode = true;
    public bool isWieldedByPlayer = true;
    public bool canFire = true;

    public WeaponStatistics WeaponStats;
    public WeaponStatistics WeaponStats_Alt;

    [Header("Inputs")]
    public KeyCode TriggerPrimary = KeyCode.Mouse0;
    public KeyCode TriggerSecondary = KeyCode.Mouse1;

    public TriggerTypes triggerType = TriggerTypes.SemiAutomatic;
    public TriggerTypes triggerType_Secondary = TriggerTypes.Sight;

    [Header("Trigger Settings: (Charge)")]
    public ChargeOptions chargeOption = ChargeOptions.Nothing;
    public float Charge_minimumTime = 0.1f;
    public float Charge_maximumTime = 1f;
    private float Charge_current;

    [Header("Variables")]
    public Transform User_POV;
    public Transform Model_Weapon;
    public Transform Origin_Barrel;
    public GameObject Prefab_Projectile;

    [Header("Components")]
    public ParticleSystem gunParticles;
    private AudioSource audioSource;

    public List<Projectile> Projectiles;
    public float fireRate_GlobalCD;

    void Start()
    {
        if (isWieldedByPlayer && User_POV == null)
            User_POV = GetComponentInChildren<Camera>().transform;

        audioSource = GetComponent<AudioSource>();


        if (!isWieldedByPlayer)
        {
            
        }            

    }

    public int frameCounter;

    void Update()
    {
        frameCounter++;

        float timeStep = Time.deltaTime;
        float projectileMaxLife = 10f;

        if(isWieldedByPlayer && canFire) // NPCs decide themselves when to run "OnFire()";
             OnFire();

        for (int i = 0; i < Projectiles.Count; i++)
        {
            Projectile currentProjectile = Projectiles[i];
            bool impacted = false;

            {
                RaycastHit hit;
                Physics.Raycast(currentProjectile.position, currentProjectile.velocity.normalized, out hit, currentProjectile.velocity.magnitude * timeStep);

                impacted = hit.transform != null;

               if(impacted)
                    OnHit(currentProjectile.projectileStats, hit.point);
            }

            if (debugMode)
            {
                float ColorGradeUp = currentProjectile.lifeTime / projectileMaxLife;
                float ColorGradeDown = 1f - ColorGradeUp;

                Color customColor = new Color(ColorGradeDown, 0, ColorGradeUp);

                Debug.DrawRay(currentProjectile.position, currentProjectile.velocity * timeStep, customColor, 1f);
            }

            currentProjectile.lifeTime += timeStep;
            currentProjectile.position += currentProjectile.velocity * timeStep;
            currentProjectile.velocity += Vector3.down * currentProjectile.gravity * timeStep;


            if (currentProjectile.visualTransform != null)
            {
                currentProjectile.visualTransform.position = currentProjectile.position;
                currentProjectile.anim.Play("Idle");
            }

           Projectiles[i] = currentProjectile;


           

            if (currentProjectile.lifeTime > projectileMaxLife || impacted)
            {
                if (currentProjectile.visualTransform != null)
                    Destroy(currentProjectile.visualTransform.gameObject); // This should actually be pooled.
                Projectiles.RemoveAt(i);
                i--;
            }
        }
    }

    #region OnFire

    /// Only TriggerType.SemiAutomtaic is supported right now.
    public void OnFire(TriggerTypes overRide = TriggerTypes.None) 
    {
        WeaponStatistics currentStats = WeaponStats;  // The Default Firemode

        fireRate_GlobalCD -= Time.deltaTime;

        bool onKeyDown = Input.GetKeyDown(TriggerPrimary);
        bool onKeyRelease = Input.GetKeyUp(TriggerPrimary);
        bool onKeyTrue = Input.GetKey(TriggerPrimary);

        triggerType = currentStats.triggerType;

        if (overRide != TriggerTypes.None)
        {
            onKeyDown = true;
            onKeyTrue = true;
        }




        if (triggerType == TriggerTypes.SemiAutomatic && !currentStats.firesInBurst)
        {
            if (onKeyDown && fireRate_GlobalCD < 0) // basic Fire
            {
                Sound.Play("Fire", Sounds, audioSource);

                fireRate_GlobalCD = 1f / currentStats.fireRate;
                CreateProjectile(currentStats);
            }

        }


        if (triggerType == TriggerTypes.SemiAutomatic && currentStats.firesInBurst)
        {
            bool remainingBursts = currentStats.burstCounter > 0;

            bool onRegularFire = fireRate_GlobalCD < 0 && onKeyDown && !remainingBursts;
            bool onBurstFire = remainingBursts && fireRate_GlobalCD < 0;

            if (onRegularFire || onBurstFire) // basic Fire
            {

                if (onRegularFire)
                    currentStats.burstCounter = currentStats.burstCount;

                Sound.Play("Fire", Sounds, audioSource);
                currentStats.burstCounter--;

                fireRate_GlobalCD = currentStats.burstCounter > 0 ? 1f / currentStats.burst_fireRate : 1f / currentStats.fireRate;
                CreateProjectile(currentStats);


            }

        }


        if (triggerType == TriggerTypes.Automatic)
        {
            if (onKeyTrue && fireRate_GlobalCD < 0) // basic Fire
            {
                Sound.Play("Fire", Sounds, audioSource);

                fireRate_GlobalCD = 1f / currentStats.fireRate;
                CreateProjectile(currentStats);
            }

        }

        if (triggerType == TriggerTypes.Charge)
        {

            if (onKeyDown && fireRate_GlobalCD < 0) // Start Charge
                Charge_current = 0;
            
            if (onKeyTrue && fireRate_GlobalCD < 0) // Charging
                Charge_current += Time.deltaTime;

            if (onKeyRelease && fireRate_GlobalCD < 0) // Release Charge
            {
                if (Charge_current < Charge_minimumTime)
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

                Charge_current = 0;
            }

        }

    }

				#endregion

				#region Projectile

    void CreateProjectile(WeaponStatistics currentStats)
    {
        if(gunParticles != null)
            gunParticles.Play();

        #region Charge Options
        float chargePercentage = (triggerType == TriggerTypes.Charge) ? Charge_current / Charge_maximumTime : 1;
        

								#endregion

								#region Hitscan
								if (currentStats.ProjectileType == ProjectileTypes.Hitscan)
        {

            for (int i = 0; i < currentStats.PelletCount; i++)
            {
                Vector3 rayDeviation = User_POV.right * Random.Range(-currentStats.Deviation, currentStats.Deviation) + User_POV.up * Random.Range(-currentStats.Deviation, currentStats.Deviation);

                Vector3 rayOrigin = User_POV.position;
                Vector3 rayDirection = User_POV.forward + rayDeviation;

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

       
                OnHit(currentStats, hit.point);

            }


        }
        #endregion

        #region Simulated Projectile

        if (currentStats.ProjectileType == ProjectileTypes.ProjectileSimulated)
        {
            float timeStep = Time.deltaTime;

            for (int i = 0; i < currentStats.PelletCount; i++)
            {
                Vector3 rayDeviation = User_POV.right * Random.Range(-currentStats.Deviation, currentStats.Deviation) + User_POV.up * Random.Range(-currentStats.Deviation, currentStats.Deviation);

                Vector3 rayOrigin = User_POV.position;
                Vector3 rayDirection = User_POV.forward + rayDeviation;

                RaycastHit hit;
                Physics.Raycast(rayOrigin, rayDirection, out hit);
                Vector3 projectileDirection = hit.transform != null ? ( hit.point - Origin_Barrel.position).normalized : (rayDirection);

                Projectile currentProjectile = new Projectile();

                currentProjectile.gravity = currentStats.Projectile_Gravity;
                currentProjectile.position = Origin_Barrel.position;
                currentProjectile.velocity = projectileDirection * currentStats.Projectile_Speed;
                currentProjectile.projectileStats = currentStats;

                if(isWieldedByPlayer && currentStats.inheritUserVelocity)
																{
                    if (currentStats.characterController == null)
                        currentStats.characterController = transform.GetComponent<Controller_Character>();

                    currentProjectile.velocity += currentStats.characterController.velocity;
                }

                if (Prefab_Projectile != null)
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


    void OnHit(WeaponStatistics currentStats, Vector3 impactPosition) // i want to skip this step at some point.
    {


        Collider[] hits = Physics.OverlapSphere(impactPosition, 0.01f);

        for (int i = 0; i < hits.Length; i++)
        {
            Transform currentHit = hits[i].transform;

            

            if (currentHit.CompareTag("Player") || currentHit.CompareTag("Enemy"))
            {
                int damage = Mathf.RoundToInt(currentStats.damage / currentStats.PelletCount);
                Component_Health.Get(currentHit).OnTakingDamage(damage, Vector3.up *  currentStats.knockback);
            }


            
        }

    }

				#endregion


    public void OnHit(string testValue)
    {
        Debug.Log(testValue + " + " + 99999 + " actual damage.");
    }
}

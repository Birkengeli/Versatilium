using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Component_Health : MonoBehaviour
{

    [Header("Settings")]
    public int HealthMax = 100;
    public int healthCurrent = -1;
    public float knockback_Multiplier = 1;

    [Header("Variables")]
    public bool isDead;

    [Header("Respawning (Player only)")]
    public float deathCountdown = 5f;
    private float deathCountdown_Timer = 0;
    public float deathCountdown_SpeedModifier = 2;
    public float deathCountdown_SpeedDuration = 0.5f;
    private float lastSpeedUse;

    [Header("Settings")]
    public Image POV_Death;

    [Header("Components")]
    Controller_Character playerScript;
    Controller_Enemy enemyScript;
    bool isPlayer { get { return enemyScript == null; } }

    // Start is called before the first frame update
    void Start()
    {
        if (healthCurrent == -1)
            healthCurrent = HealthMax;

        deathCountdown_Timer = deathCountdown;

        enemyScript = GetComponent<Controller_Enemy>();

        if(isPlayer)
            playerScript = GetComponent<Controller_Character>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead && Input.GetKeyDown(KeyCode.X)) // Suicide
            WhileDead(true);

        if (isDead)
            WhileDead(false);
    }

    public void OnTakingDamage(int damage, Vector3 knockBack)
    {



       

        if (!isDead)
        {
            healthCurrent -= damage;

            if (isPlayer)
            {
                playerScript.velocity += knockBack * knockback_Multiplier;
            }

           
        }

        if (!isDead && healthCurrent <= 0)
        {
            healthCurrent = 0;
            isDead = true;

            WhileDead(true); // On Death
        }



    }

    public static Component_Health Get(Transform target)
    {
        Component_Health healthScript = target.GetComponent<Component_Health>();

        if (healthScript == null)
            Debug.LogWarning("Could not find a healthScript.");

        return healthScript;
    }

    void WhileDead(bool onDeath)
    {
        isDead = true;

        if (isPlayer)
        {
            if (onDeath)
            {

                playerScript.StatusEffects |= Controller_Character.StatusEffect.FreezeCamera_Set;
                playerScript.StatusEffects |= Controller_Character.StatusEffect.FreezeMovement_Set;
                GetComponent<Weapon_Versatilium>().canFire = false;

                #region Visuals

                Transform eyes = transform.GetComponentInChildren<Camera>().transform;
                Vector3 cameraDeathEuler = new Vector3(-14.881f, 0.22f, 65.934f);
                Vector3 cameraDeathPosition = new Vector3(-0.509f, -0.576f, 0);
                eyes.localEulerAngles = cameraDeathEuler;
                eyes.localPosition = cameraDeathPosition;

                int childLength = eyes.childCount;
                for (int i = 0; i < childLength; i++)
                    eyes.GetChild(i).gameObject.SetActive(false);

                playerScript.velocity += -eyes.forward * 1f;

         
                #endregion
            }



            if (POV_Death != null)
            {
                POV_Death.gameObject.SetActive(true);

                float alpha = 1f - (deathCountdown_Timer / deathCountdown);

                POV_Death.color = new Color(0.8f, 0, 0, alpha);
            }

            #region Spamming buttons make you respawn faster
            if (deathCountdown_Timer >= 0)
            {
                if (Input.anyKeyDown)
                    lastSpeedUse = Time.timeSinceLevelLoad;

                bool useRespawnBoost = Time.timeSinceLevelLoad < lastSpeedUse + deathCountdown_SpeedDuration;
                deathCountdown_Timer -= Time.deltaTime * (useRespawnBoost ? deathCountdown_SpeedModifier : 1f);
            }
												#endregion

												if (deathCountdown_Timer < 0)
            {
                if (deathCountdown_Timer > -1) // On Time run out
                {
                    deathCountdown_Timer = -1;

                    Debug.Log("Respawning Player");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }

        if (!isPlayer)
        {
            enemyScript.enabled = false;
            transform.GetChild(0).position += Vector3.down * 100;
        }
    }
}

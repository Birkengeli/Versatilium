using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Trigger_Event : MonoBehaviour
{
    public enum TriggerTypes
    {
        Disabled,
        ONLYSound,
        ActivateAnotherTrigger,
        ActivateEnemies,
        ActivateBonusEnemiesForHardmode,
        ActivateRigidbodies,
        CheckPoint,
        HurtBox,
    }

    [System.Serializable]
    public class Event
    {
        [Header("Settings")]
        public TriggerTypes triggerType;
        public GameObject[] gameObjects;

        [Header("Options")]
        public float delayInSeconds = 0;
        public int Hurtbox_Damage = 1;

        [Header("Options for RigidBodies")]
        public Vector3 forceDirection;
        public float forceToAdd = 0;

        [Header("Options for Sound")]
        public bool optionalSound = false;
        public AudioSource audioSource;
        public AudioClip audioClip;
        public float volumeScale = 1;
        public bool enableLoop = false;
    }

    public Event[] Events;

    BoxCollider boxCollider;
    bool timerIsActivated;
    [HideInInspector]
    public bool hasBeenTriggered = false;

    void Awake()
    {
        transform.tag = "Event_Trigger";

        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;


    }
    void Start()
    {

        for (int i = 0; i < Events.Length; i++)
        {
            Event currentEvent = Events[i];

            if (currentEvent.delayInSeconds < 0)
                currentEvent.delayInSeconds = 0;

            if (currentEvent.triggerType == TriggerTypes.ActivateAnotherTrigger)
            {
                for (int ii = 0; ii < currentEvent.gameObjects.Length; ii++)
                {
                    currentEvent.gameObjects[ii].SetActive(false);
                }
            }

            if (currentEvent.triggerType == TriggerTypes.ActivateEnemies || currentEvent.triggerType == TriggerTypes.ActivateBonusEnemiesForHardmode)
            {
                for (int ii = 0; ii < currentEvent.gameObjects.Length; ii++)
                {
                    Controller_Enemy enemyScript = currentEvent.gameObjects[ii].GetComponent<Controller_Enemy>();

                    if (enemyScript.enemyType != Controller_Enemy.EnemyTypes.Turret) // Is it not a turret? Then don't spawn in.
                        currentEvent.gameObjects[ii].SetActive(false);

                    enemyScript.enabled = false; // Force disable
                }
            }

            if (currentEvent.triggerType == TriggerTypes.ActivateRigidbodies)
            {
                for (int ii = 0; ii < currentEvent.gameObjects.Length; ii++)
                {
                    Rigidbody currentRigidbody = currentEvent.gameObjects[ii].GetComponent<Rigidbody>();
                    currentRigidbody.isKinematic = true;
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(timerIsActivated)
            Events_Trigger(Time.deltaTime);
    }

				private void OnTriggerEnter(Collider other)
				{
        if (!other.CompareTag("Player"))
            return;

        print("Trigger entered");

        Events_Trigger(0, other.transform);
        boxCollider.enabled = false;
    }

    void Events_Trigger(float timeStep, Transform player = null)
    {
        hasBeenTriggered = true;
        timerIsActivated = false;

        for (int i = 0; i < Events.Length; i++)
        {
            Event currentEvent = Events[i];

												#region One Time trigger, and delay
												if (currentEvent.delayInSeconds == -1)
                continue;

            if (currentEvent.delayInSeconds > 0)
            {
                timerIsActivated = true;

                currentEvent.delayInSeconds -= Time.deltaTime;
                continue;
            }
            
            currentEvent.delayInSeconds = -1; // It will be triggered once, and never again.

            #endregion


            if (currentEvent.optionalSound && currentEvent.triggerType != TriggerTypes.Disabled)
            {
                if (!currentEvent.enableLoop)
                    currentEvent.audioSource.PlayOneShot(currentEvent.audioClip, currentEvent.volumeScale);
                if (currentEvent.enableLoop)
                {
                    currentEvent.audioSource.clip = currentEvent.audioClip;
                    currentEvent.audioSource.volume = currentEvent.volumeScale;
                    currentEvent.audioSource.loop = currentEvent.enableLoop;
                    currentEvent.audioSource.Play();
                }
            }

            if (currentEvent.triggerType == TriggerTypes.ActivateAnotherTrigger)
            {
                for (int ii = 0; ii < currentEvent.gameObjects.Length; ii++)
                    currentEvent.gameObjects[ii].SetActive(true);
            }

            if (currentEvent.triggerType == TriggerTypes.ActivateEnemies)
            {
                for (int ii = 0; ii < currentEvent.gameObjects.Length; ii++)
                {
                    currentEvent.gameObjects[ii].SetActive(true); // Force Active
                    currentEvent.gameObjects[ii].GetComponent<Controller_Enemy>().enabled = true;
                }
            }

            if (currentEvent.triggerType == TriggerTypes.ActivateRigidbodies)
            {
                for (int ii = 0; ii < currentEvent.gameObjects.Length; ii++)
                {
                    Rigidbody currentRigidbody = currentEvent.gameObjects[ii].GetComponent<Rigidbody>();
                    currentRigidbody.isKinematic = false;

                    if (currentEvent.forceToAdd != 0)
                        currentRigidbody.velocity = currentEvent.forceDirection.normalized * currentEvent.forceToAdd;   
                }
            }

            if (currentEvent.triggerType == TriggerTypes.CheckPoint && player != null)
            {
                Transform playerEyes = player.GetComponentInChildren<Camera>().transform;
                Component_Health healthScript = player.GetComponent<Component_Health>();

                Vector3 playerPosition = player.position;
                Vector2 playerEuler = new Vector2(playerEyes.localEulerAngles.x, player.localEulerAngles.y);

                float playerHealth = Mathf.Max(healthScript.healthCurrent, 1); // You always have 1 hp. Mostly for crossing a checkpoint while dead.

                /// Save Triggered Checkpoints
                /// Disables them after re-triggering them. Does not play sounds, does not respawn enemies, does not save other checkpoints.
                /// Triggers them without the timers
                /// I am realising I must save them on the player. I am not triggering a quadzillion triggers every frame.
                /// 


                GameObject[] triggers = GameObject.FindGameObjectsWithTag("Trigger_Event");

                int triggered_Array_Length = 0;

                for (int ii = 0; ii < triggers.Length; ii++)
                    triggered_Array_Length += triggers[ii].GetComponent<Trigger_Event>().hasBeenTriggered ? 1 : 0;

                for (int ii = 0; ii < currentEvent.gameObjects.Length; ii++)
                    currentEvent.gameObjects[ii].SetActive(true);

                /// Saves Weapon Configs
                /// 



            }

            if (currentEvent.triggerType == TriggerTypes.HurtBox && player != null)
            {
                player.GetComponent<Component_Health>().OnTakingDamage(currentEvent.Hurtbox_Damage, currentEvent.forceDirection.normalized * currentEvent.forceToAdd);
            }
        }

        if (!timerIsActivated) // If there is no timer, or the timer is up, disable the whole thing.
            gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Pickup : MonoBehaviour
{

    public enum Pickup
				{
        GiveVersatilium, RemoveVersatilium, PickOneOfTwo
				}

    public bool debugMode = true;
    float orbitTimer = 0;

    [Header("Pickup")]
    public Pickup pickupType = Pickup.PickOneOfTwo;
    public Weapon_Versatilium.WeaponStatistics Option_A;
    public Weapon_Versatilium.WeaponStatistics Option_B;

    [Header("Settings")]
    public float distance = 2f;

    bool isActive;
    Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        isActive = playerTransform != null;
    }

    void Update()
    {
        Vector3 center = transform.position;

        if (debugMode)
        {
            orbitTimer += Time.deltaTime * 360;

            Vector3 sideWay = Quaternion.AngleAxis(orbitTimer, Vector3.up) * Vector3.right;
            Debug.DrawRay(center, sideWay * distance, Color.red, Time.deltaTime);
        }

        if(isActive && Vector3.Distance(center, playerTransform.position) < distance)
								{
            // On Pickup

            OnPickup();

								}
    }


    void OnPickup()
    {
        if (pickupType == Pickup.GiveVersatilium || pickupType == Pickup.RemoveVersatilium)
        {
            playerTransform.GetComponent<Weapon_Versatilium>().enabled = pickupType == Pickup.GiveVersatilium;
        }

        if (pickupType == Pickup.PickOneOfTwo)
        {
            Weapon_Versatilium weapon = playerTransform.GetComponent<Weapon_Versatilium>();
        }




    }
}

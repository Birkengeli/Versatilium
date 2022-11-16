using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Arsenal : MonoBehaviour
{

    public enum SlotType
    {
        Pistol = 0,
        Shotgun = 1,
        Rifle = 2,
        Plasma = 3,

    }

    [System.Serializable]
    public class WeaponConfiguration
    {
        public string name;
        public bool isUnlocked;
        public int weaponWheelIndex = -1;

        public SlotType WeaponSlot;

        public Weapon_Versatilium.WeaponStatistics statistics;
    }

    public WeaponConfiguration[] weaponConfigs;


    [Header("Settings")]
    public bool useMouseWheel = false;
    public float switchCooldown = 1;
    private float switchCooldown_Timer;
    public AudioClip switchSound;

    private int weaponCurrentIndex = 0;

    Weapon_Versatilium Versatilium;

    void Start()
    {
        Versatilium = GetComponent<Weapon_Versatilium>();

        Versatilium.WeaponStats = weaponConfigs[weaponCurrentIndex].statistics;
        switchCooldown_Timer = switchCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        int scrollDirection = Input.GetAxis("Mouse ScrollWheel") > 0 ? 1 : 0 + Input.GetAxis("Mouse ScrollWheel") < 0 ? -1 : 0;

        if (useMouseWheel && scrollDirection != 0 && switchCooldown_Timer == -1)
        {

            while (true)
            {
                weaponCurrentIndex += scrollDirection;

                if (weaponCurrentIndex == weaponConfigs.Length)
                    weaponCurrentIndex = 0;

                if (weaponCurrentIndex == -1)
                    weaponCurrentIndex = weaponConfigs.Length - 1;

                if (weaponConfigs[0].isUnlocked == false)
                {
                    weaponConfigs[0].isUnlocked = true;
                    Debug.LogWarning("Please do not lock the pistol, if there is no available weapons the code breaks.");
                }

                if(weaponConfigs[weaponCurrentIndex].isUnlocked)
                    break;
            }


            SwitchWeapon(weaponConfigs[weaponCurrentIndex]);
        }


        if (switchCooldown_Timer > 0)
            switchCooldown_Timer -= Time.deltaTime;
        else
            switchCooldown_Timer = -1;
    }

    public void SwitchWeapon(WeaponConfiguration switchToConfig)
    {

        if (Versatilium.WeaponStats == switchToConfig.statistics)
            return;


        Versatilium.WeaponStats = switchToConfig.statistics;
        switchCooldown_Timer = switchCooldown;


        GetComponent<AudioSource>().PlayOneShot(switchSound);
    }

    public void SwitchWeaponUsingWheel(int index)
    {

        for (int i = 0; i < weaponConfigs.Length; i++)
        {
            if (weaponConfigs[i].weaponWheelIndex == index)
            {
                SwitchWeapon(weaponConfigs[i]);
                return;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sounds_Turret
{
    Unassigned,

    OnFire,
    OnActivate,
    OnRetract,
}

public enum Sounds_Weapon
{
    Unassigned,

    OnFire,
    OnFire_Charged,
    OnReload,
}
[System.Serializable]
public class Manager_Audio
{
    [Header("You usually only pick one of these")]
    public Sounds_Turret Turret;
    public Sounds_Turret Weapon;

    [Header("Example: 'event:/Character/Enemies/Turret_Small/Turret Retract'")]
    public string audioPath = "event:/Character/Enemies/Turret_Small/Turret Retract";


    public static void Play(Manager_Audio[] sounds, Sounds_Turret soundType)
				{
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].Turret == soundType)
            {
                FMODUnity.RuntimeManager.PlayOneShot(sounds[i].audioPath);

                return;
            }
        }
    }
}

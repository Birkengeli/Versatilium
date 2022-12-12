using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sounds_Turret
{
    Unassigned,

    OnFire,
    OnActivate,
    OnRetract,
    OnDeath,
}

public enum Sounds_Generic
{
    Unassigned,

    Footsteps,
    ButtonPress,
}

public enum Sounds_Weapon
{
    Unassigned,

    Pistol_OnFire,
    Rifle_OnFire,
    Shotgun_OnFire,
}

[System.Serializable]
public class Manager_Audio
{
    [Header("You usually only pick one of these")]
    public Sounds_Turret Turret;
    public Sounds_Generic Generic;
    public Sounds_Weapon Weapon;

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

    public static void Play(Manager_Audio[] sounds, Sounds_Generic soundType)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].Generic == soundType)
            {
                FMODUnity.RuntimeManager.PlayOneShot(sounds[i].audioPath);

                return;
            }
        }
    }

    public static void Play(Manager_Audio[] sounds, Sounds_Weapon soundType)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].Weapon == soundType)
            {
                FMODUnity.RuntimeManager.PlayOneShot(sounds[i].audioPath);

                return;
            }
        }
    }
}

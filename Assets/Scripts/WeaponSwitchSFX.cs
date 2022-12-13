using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitchSFX : MonoBehaviour
{
    private FMOD.Studio.EventInstance instance;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            instance = FMODUnity.RuntimeManager.CreateInstance("snapshot:/Weapon Select");
            instance.start();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.release();
        }
    }
}
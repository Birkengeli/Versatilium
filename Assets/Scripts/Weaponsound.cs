using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weaponsound : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Weapons/Weapons Parameter");
        }
    }
}
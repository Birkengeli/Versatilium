// Code by Creepy Cat (C) 2021/2022
// Code given for example! 
// You need to modify by yourself for your needs...
//
// IF you improve the code, do not hesitate to send me! (credited to the updates) 
// black.creepy.cat@gmail.com 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace creepycat.scifikitvol4
{
    // A classe to manage some aspects of the kit
    public class KitManager : MonoBehaviour{

    [Header("")]
    [Tooltip("World setup")]
    public float worldGravity = -11.0f; // Default gravity
    public bool useZeroGravity = false;

    [Header("")]
    [Tooltip("Select the gravity zero key")]
    public KeyCode switchGravity = KeyCode.G;

    [Header("")]
    [Tooltip("Select the gravity switch sounds")]
    public AudioClip GravitySwitchSound;
    public AudioClip GravityOnSound;
    public AudioClip GravityOffSound;
    
    // For cat only
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start(){
        audioSource = GetComponent<AudioSource>();
        ChangeGravity();
    }

    void ChangeGravity(){
        if(useZeroGravity == false){
            Physics.gravity = new Vector3(0, worldGravity, 0);
            audioSource.PlayOneShot(GravityOffSound);
            audioSource.PlayOneShot(GravitySwitchSound); 
        }else{
            Physics.gravity = new Vector3(0, 0, 0);
            audioSource.PlayOneShot(GravityOnSound);
            audioSource.PlayOneShot(GravitySwitchSound);
        }
    }

    // Update is called once per frame
    void Update(){

       // If you want to play with gravity :)
       if (Input.GetKeyDown(switchGravity)){
            useZeroGravity=!useZeroGravity;
            ChangeGravity();
       }

       // Stuff about cursor...
       Cursor.lockState = CursorLockMode.Locked;
       Cursor.visible = false;
    }
}

}
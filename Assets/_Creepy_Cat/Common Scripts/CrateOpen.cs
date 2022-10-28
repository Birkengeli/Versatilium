// Code by Creepy Cat (C) 2021/2022
// Code given for example! 
// You need to modify by yourself for your needs...
//
// IF you improve the code, do not hesitate to send me! (credited to the updates) 
// black.creepy.cat@gmail.com 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Uween;

namespace creepycat.scifikitvol4
{

    public class CrateOpen : MonoBehaviour
    {
        public GameObject CrateTop;
        public GameObject CrateButton;
        public GameObject CrateLight;

        public AudioClip CrateSound;

        public float OpenTime = 2.0f;

        public float EmissionIntensity = 1.2f;

        private bool SwitchAnim = false;
        private float RotateMax = -90f;

        private Renderer ButtonRenderer;
        private AudioSource AudioSource;

        private bool AnimationFlag = false;

        void Start(){
            ButtonRenderer = CrateButton.GetComponent<Renderer>();
            AudioSource = GetComponent<AudioSource>();
        }

        void EndAnimationFlag(){
            AnimationFlag = false;
        }

        // Update is called once per frame    
        void Update(){
            // If mouse click
            if (Input.GetMouseButtonDown(0))
            {
                // Get the gameobject clicked
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // If something clicked
                if (Physics.Raycast(ray, out hit))
                {

                    // If it's my button
                    if (hit.transform == CrateButton.transform)
                    {
                        if (AnimationFlag == false)
                        {
                            SwitchAnim = !SwitchAnim;
                            AnimationFlag = true;

                            // Anim switch var
                            switch (SwitchAnim)
                            {
                                // If no we launch all the things needed
                                case false:
                                    TweenRX.Add(CrateTop, OpenTime, 0).From(RotateMax).EaseInOutCubic().Then(EndAnimationFlag);
                                    ButtonRenderer.material.SetColor("_EmissionColor", Color.white * EmissionIntensity);

                                    CrateLight.SetActive(true);
                                    AudioSource.PlayOneShot(CrateSound, 1.0F);
                                    break;

                                case true:
                                    TweenRX.Add(CrateTop, OpenTime, RotateMax).Relative().EaseInOutCubic().Then(EndAnimationFlag);
                                    ButtonRenderer.material.SetColor("_EmissionColor", Color.red * EmissionIntensity);

                                    CrateLight.SetActive(false);
                                    AudioSource.PlayOneShot(CrateSound, 1.0F);
                                    break;
                            }
                        }
                    }

                }
            }
        }

    }
}
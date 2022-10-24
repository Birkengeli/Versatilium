using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dev_Cheats : MonoBehaviour
{

    public int codeIndex = 0;

    public class Cheats
    {
        public string name;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
								if (Input.anyKeyDown)
        {

            if (Input.GetKeyDown(KeyCode.D) && (codeIndex == 0 || codeIndex == 1))
            {
                codeIndex = 1;
                return;
            }

            if (Input.GetKeyDown(KeyCode.O) && (codeIndex == 1 || codeIndex == 2))
            {
                codeIndex++;
                return;
            }
            if (Input.GetKeyDown(KeyCode.M) && codeIndex == 3)
            {
   
                // Change scene to secret level.
                Debug.Log("Cheat Code Activated: 'Doom'.");
                codeIndex = 0;

                return;
            }


            codeIndex = 0;
        }
    }
}

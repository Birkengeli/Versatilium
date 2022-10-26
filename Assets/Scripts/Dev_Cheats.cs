using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dev_Cheats : MonoBehaviour
{

    public int codeIndex = 0;
    [System.Serializable]
    public class Cheat
    {
        public string name;
        public KeyCode[] keyCodes;
        public int keyCodeIndex = 0;
    }

    public Cheat[] Cheats;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Cheats.Length; i++)
            Cheats[i].keyCodes = StringToKeyCode(Cheats[i].name.ToUpper());
        
    }

    // Update is called once per frame
    void Update()
    {
								if (Input.anyKeyDown)
        {
            for (int i = 0; i < Cheats.Length; i++)
            {
                RecogniseKey(Cheats[i]);
            }
        }

    }

    void RecogniseKey(Cheat currentCheat)
    {
        KeyCode currentKeyCode = currentCheat.keyCodes[currentCheat.keyCodeIndex];

        if (Input.GetKeyDown(currentKeyCode))
        {
            currentCheat.keyCodeIndex++;
        }
        else
            currentCheat.keyCodeIndex = 0;

        if (currentCheat.keyCodeIndex == currentCheat.keyCodes.Length)
        {
            CodeCompleted(currentCheat.name.ToLower());
            currentCheat.keyCodeIndex = 0;
        }
    }

    KeyCode[] StringToKeyCode(string message)
    {
        KeyCode[] keyCodes = new KeyCode[message.Length];

        for (int i = 0; i < message.Length; i++)
        {
            keyCodes[i] = (KeyCode)System.Enum.Parse(typeof(KeyCode), ("" + message[i]));
        }

        return keyCodes;
    }

    public void CodeCompleted(string name)
    {
        print("Code Activated: '" + name + "'.");

        if (name == "kill")
        {
            GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Component_Health>().WhileDead(true);


        }
    }
}

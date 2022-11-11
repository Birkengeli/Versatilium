using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon_Switching : MonoBehaviour
{
    #region Enums

    public enum WeaponBuildingModes
    {
        Premade, Modular
    }

    #endregion

    Controller_Character playerScript;
				Weapon_Versatilium weaponScript;
    Weapon_Arsenal arsenalScript;

    GameObject canvas;
    Image tint;


    [Header("Settings")]
    public WeaponBuildingModes WeaponBuildingMode = WeaponBuildingModes.Premade;
    public KeyCode InventoryKey = KeyCode.Tab;

    [Header("Settings - Visuals")]
    public int SlowTimeBy = 10;
    public Color tintColor = Color.blue;
    public float tintAlphaOverride = 0.5f;
    public float tintFadeTime = 1;

    #region Start & Update
    void Start()
    {
        playerScript = GetComponent<Controller_Character>();
        weaponScript = GetComponent<Weapon_Versatilium>();
        arsenalScript = GetComponent<Weapon_Arsenal>();

        canvas = GameObject.Find("_Canvas");
        if (canvas == null)
            Debug.LogWarning("Could not find the '_Canvas' prefab");

        Transform[] UI_Elements = canvas.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < UI_Elements.Length; i++)
        {
            Transform currentElement = UI_Elements[i];

            if(currentElement.name == "UI_Tint")
                tint = currentElement.GetComponent<Image>();
        }

        tintColor.a = tintAlphaOverride;
        Color tintClear = tintColor;
        tintClear.a = 0;
        tint.CrossFadeColor(tintClear, tintFadeTime, true, true);
    }

    // Update is called once per frame
    void Update()
    {
        bool onInventoryDown = Input.GetKeyDown(InventoryKey);
        bool WhileInventoryDown = Input.GetKey(InventoryKey);
        bool onInventoryRelease = Input.GetKeyUp(InventoryKey);

        if (onInventoryDown || WhileInventoryDown || onInventoryRelease)
            OpenInventory((onInventoryDown ? 1 : 0) + (onInventoryRelease ? 2 : 0));
    }

				#endregion

				void OpenInventory(int keyState)
    {
        if (WeaponBuildingMode == WeaponBuildingModes.Premade)
        {
            if (keyState == 1)
            {
                Controller_Spectator.LockCursor(false);
                Time.timeScale = (1f / SlowTimeBy);
                playerScript.ApplyStatusEffect(Controller_Character.StatusEffect.FreezeCamera);
                playerScript.ApplyStatusEffect(Controller_Character.StatusEffect.DisableShooting);

                tint.gameObject.SetActive(true);
                tintColor.a = tintAlphaOverride;
                tint.color = tintColor;
                tint.CrossFadeColor(tintColor, tintFadeTime, true, true);


            }




            if (keyState == 2)
            {
                Controller_Spectator.LockCursor(true);
                Time.timeScale = 1;
                playerScript.ApplyStatusEffect(Controller_Character.StatusEffect.FreezeCamera, true);
                playerScript.ApplyStatusEffect(Controller_Character.StatusEffect.DisableShooting, true);

                Color tintClear = tintColor;
                tintClear.a = 0;
                tint.CrossFadeColor(tintClear, tintFadeTime, true, true);
            }
        }
    }
}

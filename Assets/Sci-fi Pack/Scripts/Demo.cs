using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour {

  
    public GameObject[] FXList;
    public GameObject[] LaserList;
    public Text Title;
    public int Selection = 0;

    public GameObject BackText;
    public GameObject NextText;
    public GameObject BackButton;
    public GameObject NextButton;

    public bool Block = false;
    public bool LaserMode = false;
    void Start () {
        FXList[Selection].SetActive(true);
        Title.text = "Prefab Name: " + FXList[Selection].gameObject.transform.name.ToString();
    }

	void Update()
    {
        if (Block == false)
        {
            if (Selection == 0)
            {
                BackText.SetActive(false);
                BackButton.SetActive(false);

            }
            else
            {
                BackText.SetActive(true);
                BackButton.SetActive(true);

            }

            if (Selection == FXList.Length - 1 && LaserMode == false)
            {
                NextText.SetActive(false);
                NextButton.SetActive(false);

            }
            if (Selection != FXList.Length - 1 && LaserMode == false)
            {
                NextText.SetActive(true);
                NextButton.SetActive(true);

            }

            if (Selection == LaserList.Length - 1 && LaserMode)
            {
                NextText.SetActive(false);
                NextButton.SetActive(false);

            }
            if (Selection != LaserList.Length - 1 && LaserMode)
            {
                NextText.SetActive(true);
                NextButton.SetActive(true);

            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Next();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                Back();
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Canvas C;
            C = GetComponent<Canvas>();
            C.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Canvas C;
            C = GetComponent<Canvas>();
            C.enabled = true;
        }

    }
    public void Back()
    {
       
        if (Selection < FXList.Length && Selection != 0 && LaserMode == false)
        {
            FXList[Selection].SetActive(false);
            Selection -= 1;
            FXList[Selection].SetActive(true);
            Title.text = "Prefab Name: " + FXList[Selection].gameObject.transform.name.ToString();
        }

        if (Selection < LaserList.Length && Selection != 0 && LaserMode)
        {
            LaserList[Selection].SetActive(false);
            Selection -= 1;
            LaserList[Selection].SetActive(true);
            Title.text = "Prefab Name: " + LaserList[Selection].gameObject.transform.name.ToString();
        }

    }

    public void Next()
    {
       
        if(Selection < FXList.Length && Selection != FXList.Length - 1 && LaserMode == false)
        {
            FXList[Selection].SetActive(false);
            Selection += 1;
            FXList[Selection].SetActive(true);
            Title.text = "Prefab Name: " + FXList[Selection].gameObject.transform.name.ToString();
        }

        if (Selection < LaserList.Length && Selection != LaserList.Length - 1 && LaserMode)
        {
            LaserList[Selection].SetActive(false);
            Selection += 1;
            LaserList[Selection].SetActive(true);
            Title.text = "Prefab Name: " + LaserList[Selection].gameObject.transform.name.ToString();
        }

    }

    public void Last()
    {
        if (Selection < FXList.Length)
        {
            for(var i = 0; i < FXList.Length; i++)
            {
                FXList[i].SetActive(false);
            }
          
            Selection = 0;
            FXList[Selection].SetActive(true);
            Title.text = "Prefab Name: " + FXList[Selection].gameObject.transform.name.ToString();
        }

        if (Selection < LaserList.Length && LaserMode)
        {
            for (var i = 0; i < LaserList.Length; i++)
            {
                LaserList[i].SetActive(false);
            }

            LaserList[Selection].SetActive(false);
            Selection = 0;
            LaserList[Selection].SetActive(true);
            Title.text = "Prefab Name: " + LaserList[Selection].gameObject.transform.name.ToString();
        }

    }
    public void Effects()
    {
        
        Block = false;
        LaserMode = false;
    }
    public void Lasers()
    {
       
        Block = false;
        LaserMode = true;
    }
    public void Projectiles()
    {
        
        Block = true;
        LaserMode = false;
    }

}

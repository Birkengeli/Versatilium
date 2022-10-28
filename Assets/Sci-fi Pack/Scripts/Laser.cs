using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    LineRenderer line;
    List<Vector3> pos = new List<Vector3>(1);
    public GameObject Impact;
    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        pos.Add(new Vector3 (0,0,0));
        pos.Add(new Vector3(1, 1, 1));
    }

   
    void Update()
    {
        RaycastHit Ray;
        
         if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out Ray))
         {

             pos[0] = transform.InverseTransformPoint(transform.position);
             pos[1] = transform.InverseTransformPoint(Ray.point);
             line.SetPosition(0, pos[0]);
             line.SetPosition(1, pos[1]);
              if (Impact)
               {
                Impact.transform.position = Ray.point;
               }
             
             Debug.DrawLine(transform.position, Ray.point);

         }
         
        Ray MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit MouseHit;
        if (Physics.Raycast(MouseRay, out MouseHit))
        {
            transform.LookAt(MouseHit.point);
        }
    }
}

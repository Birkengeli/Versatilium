using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject Explosion;
    public float Timer = 3f;
    public float TimerEXP = 0.5f;

    public bool Detach = false;
    void Start()
    {
        Destroy(gameObject, Timer);
    }

    void Exp()
    {
        if (Detach)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject, 1f);
                child.parent = null;
            }
        }
        Destroy(gameObject);
        GameObject E = Instantiate(Explosion, transform.position, Explosion.transform.rotation);
        Destroy(E, TimerEXP);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Exp();
    }
    
 
}

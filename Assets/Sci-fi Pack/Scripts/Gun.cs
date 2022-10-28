using UnityEngine;
using UnityEngine.UI;


public class Gun : MonoBehaviour
{
    public Transform Launcher;

    public GameObject[] FlashPrefab;
    public Rigidbody[] Projectile;
    public float[] Speed;
    public float FireRate = 0.1f;

    public Camera Camera;
    float NextFire = 0.0f;
    public float mouseSensitivity = 2.0f;


    private float rotY = 0.0f;
    private float rotX = 0.0f;
    public float clampAngle = 80.0f;

    public int Selection = 0;

    public Text PrefabName;
    Vector3 Point;
    Animator Anim;
    void Start()
    {
        Anim = GetComponent<Animator>();

        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Screen.lockCursor = true;
    }

    private void Update()
    {
        if (PrefabName) {
            PrefabName.text = "Prefab Name: " + Projectile[Selection].name.ToString();
        }
       
        if (Input.GetKeyDown(KeyCode.D) && Selection < Projectile.Length - 1)
        {
            Selection += 1;
        }

        if (Input.GetKeyDown(KeyCode.A) && Selection > 0)
        {
            Selection -= 1;
        }
    }

    void FixedUpdate()
    {
        var v = Input.GetAxis("Mouse Y");
        var h = Input.GetAxis("Mouse X");

        rotY += v * mouseSensitivity * Time.deltaTime;
        rotX += h * mouseSensitivity * Time.deltaTime;


        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        rotY = Mathf.Clamp(rotY, -70, 70);

        Quaternion localRotation = Quaternion.Euler(-rotY, rotX, 0.0f);

        transform.rotation = localRotation;

       

        if (Input.GetKey(KeyCode.Escape))
        {
            Screen.lockCursor = false;
           // Time.timeScale = 0;
        }


        
        if (Input.GetButton("Fire1") && Time.time > NextFire)
        {
            
            NextFire = FireRate + Time.time;
            Anim.Play("Shoot", 0);

            GameObject F;
            F = Instantiate(FlashPrefab[Selection], Launcher.transform.position, FlashPrefab[Selection].transform.rotation);
            F.transform.SetParent(Launcher);
            Destroy(F, 1f);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
            {
               Point = new Vector3(hit.point.x + Random.Range(-0.2f, 0.2f), 
                   hit.point.y + Random.Range(-0.2f, 0.2f), hit.point.z); ;
               
                Debug.DrawLine(Launcher.position, Point, Color.red);
            }

            Rigidbody P;
            var Rel = (Point - Launcher.position).normalized;
            P = Instantiate(Projectile[Selection], Launcher.position, Projectile[Selection].transform.rotation);
            P.velocity = Rel * Speed[Selection];
        }

        if (Time.time > NextFire)
        {
            Anim.Play("Default", 0);
        }


    }

    private void OnEnable()
    {
        Screen.lockCursor = true;
        Selection = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float StaminaMax = 3;
    private float Stamina;
    private bool exhausted;
    private float exhaustedT = 0;
    public float WalkSpeed;
    public float RunSpeed;
    private bool running = false;
    private float ZInput = 0;
    private float XInput = 0;
    

    private Transform pos;
    private Rigidbody rb;

    private bool Rolling = false;
    public float DashA = 0.05f;
    private Vector3 direct = new Vector3(0,0,0);
    public float length = 1;
    private float DTimer = 0;
    private float topS = 0; 
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pos = GetComponent<Transform>();
        Stamina = StaminaMax;
    }

    private void FixedUpdate()
    {
        if (Rolling == false)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.velocity += transform.forward * XInput * (running == false ? WalkSpeed : RunSpeed);
            rb.velocity += transform.right * ZInput * (running == false ? WalkSpeed : RunSpeed);
        }

        if (Rolling == true)
        {
            //print("Z =" + direct.z / Mathf.Abs(direct.z)); 
            //print("X =" + direct.x / Mathf.Abs(direct.x));
            //adds velocity in the direction the player was already moving when dashing. Queries if the velocity is 0 to prevent errors
            rb.velocity += new Vector3( direct.x != 0 ? DashA * Mathf.Sign(direct.x): 0, 0, direct.z != 0 ? DashA * Mathf.Sign(direct.z): 0);
        }
    }
    void Update()
    {
        if (Rolling == false)
        {
            DTimer = 0;
            ZInput = Input.GetAxis("Horizontal");
            XInput = Input.GetAxis("Vertical");
        }
        //start dash
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rolling = true;
            direct = rb.velocity;
        }
        if (Rolling == true)
        {
            DTimer += Time.deltaTime;
            if (DTimer >= length)
            {
                Rolling = false;
            }
        }
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && exhausted == false)
        {
            running = true;
            Stamina -= Time.deltaTime;
        } else
        {
            running = false;
            Stamina += Time.deltaTime;
        }

        if (Stamina <= 0)
        {
            exhausted = true;
            exhaustedT = StaminaMax;
        }
        if (Stamina >= StaminaMax)
        {
            Stamina = StaminaMax;
        }
        if (exhausted == true)
        {
            exhaustedT -= Time.deltaTime;
            if (exhaustedT <= 0)
            {
                exhausted = false;
            }
        }
        print("Exhausted?" + exhausted);
        print("Stamina =" + Stamina);
    }
}

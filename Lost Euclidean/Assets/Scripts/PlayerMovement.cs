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
    private Vector3 direct = new Vector3(0, 0, 0);
    public float length = 1;
    private float DTimer = 0;
    // private float topS = 0; 
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
            rb.velocity += new Vector3(direct.x != 0 ? DashA * Mathf.Sign(direct.x) : 0, 0, direct.z != 0 ? DashA * Mathf.Sign(direct.z) : 0);
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
        }
        else
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
    }

    private void OnTriggerExit(Collider other)
    {
        //teleport code
        if (other.tag == "Room")
        {
            //get player exit direction
            Vector3 newPos = pos.position - other.transform.position;
            int direction = 0;
            if (Mathf.Abs(newPos.x) > Mathf.Abs(newPos.z))
            {
                if (newPos.x < 0)
                {
                    direction = 0;
                }
                else
                {
                    direction = 1;
                }
            }
            else if (Mathf.Abs(newPos.x) < Mathf.Abs(newPos.z))
            {
                if (newPos.z < 0)
                {
                    direction = 2;
                }
                else
                {
                    direction = 3;
                }
            }

            //teleport player
            if (direction == 0) //west exit
            {
                // Debug.Log("west: " + RoomManager.instance.GetCurrentRoom().roomCoords.x + "," + RoomManager.instance.GetCurrentRoom().roomCoords.y);
                var room = RoomManager.instance.GetRandomEastRoom();
                pos.position = new Vector3(room.transform.position.x + 7.5f, pos.position.y, room.transform.position.z);
                // Debug.Log("go east: " + room.roomCoords.x + "," + room.roomCoords.y);
            }
            if (direction == 1) //east exit
            {
                // Debug.Log("east: " + RoomManager.instance.GetCurrentRoom().roomCoords.x + "," + RoomManager.instance.GetCurrentRoom().roomCoords.y);
                var room = RoomManager.instance.GetRandomWestRoom();
                pos.position = new Vector3(room.transform.position.x - 7.5f, pos.position.y, room.transform.position.z);
                // Debug.Log("go west: " + room.roomCoords.x + "," + room.roomCoords.y);
            }
            if (direction == 2) //south exit
            {
                // Debug.Log("south: " + RoomManager.instance.GetCurrentRoom().roomCoords.x + "," + RoomManager.instance.GetCurrentRoom().roomCoords.y);
                var room = RoomManager.instance.GetRandomNorthRoom();
                pos.position = new Vector3(room.transform.position.x, pos.position.y, room.transform.position.z + 3.5f);
                // Debug.Log("go north: " + room.roomCoords.x + "," + room.roomCoords.y);
            }
            if (direction == 3) //north exit
            {
                // Debug.Log("north: " + RoomManager.instance.GetCurrentRoom().roomCoords.x + "," + RoomManager.instance.GetCurrentRoom().roomCoords.y);
                var room = RoomManager.instance.GetRandomSouthRoom();
                pos.position = new Vector3(room.transform.position.x, pos.position.y, room.transform.position.z - 3.5f);
                // Debug.Log("go south: " + room.roomCoords.x + "," + room.roomCoords.y);
            }
        }
    }
}

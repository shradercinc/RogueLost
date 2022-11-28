using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public Camera cam;
    private Vector3 mPos;

    private bool Rolling = false;
    public float DashA = 0.05f;
    private Vector3 direct = new Vector3(0, 0, 0);
    public float length = 1;
    private float DTimer = 0;
    // private float topS = 0; 

    private bool exitStairs = false;



    void Start()
    {
        mPos = Input.mousePosition;
        rb = GetComponent<Rigidbody>();
        pos = GetComponent<Transform>();
        Stamina = StaminaMax;
        cam = Camera.main;
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
        mPos = Input.mousePosition;
        var pPos = cam.WorldToScreenPoint(pos.position);
        Vector3 dir = (mPos - pPos);
        dir.z = dir.y;
        dir.y = 0f;
        dir.Normalize();
        transform.rotation = Quaternion.LookRotation(dir);



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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Exit")
        {
            if (GameManager.instance.isTeleporting)
            {
                //go to starting room
                TeleportToCenter(RoomManager.instance.GetStartingRoom());
            }
            else
            {
                //end game
                SceneManager.LoadScene(1);//end scene
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //teleport code
        if (other.tag == "Room" && exitStairs == false)
        {
            if (GameManager.instance.isTeleporting)
            {
                Teleport(other);
            }
        }
    }
    private void TeleportToCenter(Room room)
    {
        exitStairs = true; //TODO: might break
        Vector3 newPos = room.gameObject.transform.position;
        pos.position = newPos;
        Invoke("ResetExit", 1);
    }

    private void ResetExit()
    {
        exitStairs = false;
    }

    private void Teleport(Collider roomCollider)
    {
        //get player exit direction
        Vector3 newPos = pos.position - roomCollider.transform.position;
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
            var room = RoomManager.instance.GetCurrentRoom().nsewRooms.east;
            pos.position = new Vector3(room.transform.position.x + 7.5f, pos.position.y, room.transform.position.z);
            // Debug.Log("go east: " + room.roomCoords.x + "," + room.roomCoords.y);
        }
        if (direction == 1) //east exit
        {
            // Debug.Log("east: " + RoomManager.instance.GetCurrentRoom().roomCoords.x + "," + RoomManager.instance.GetCurrentRoom().roomCoords.y);
            // var room = RoomManager.instance.GetRandomWestRoom();
            var room = RoomManager.instance.GetCurrentRoom().nsewRooms.west;
            pos.position = new Vector3(room.transform.position.x - 7.5f, pos.position.y, room.transform.position.z);
            // Debug.Log("go west: " + room.roomCoords.x + "," + room.roomCoords.y);
        }
        if (direction == 2) //south exit
        {
            // Debug.Log("south: " + RoomManager.instance.GetCurrentRoom().roomCoords.x + "," + RoomManager.instance.GetCurrentRoom().roomCoords.y);
            // var room = RoomManager.instance.GetRandomNorthRoom();
            var room = RoomManager.instance.GetCurrentRoom().nsewRooms.north;
            pos.position = new Vector3(room.transform.position.x, pos.position.y, room.transform.position.z + 3.5f);
            // Debug.Log("go north: " + room.roomCoords.x + "," + room.roomCoords.y);
        }
        if (direction == 3) //north exit
        {
            // Debug.Log("north: " + RoomManager.instance.GetCurrentRoom().roomCoords.x + "," + RoomManager.instance.GetCurrentRoom().roomCoords.y);
            // var room = RoomManager.instance.GetRandomSouthRoom();
            var room = RoomManager.instance.GetCurrentRoom().nsewRooms.south;
            pos.position = new Vector3(room.transform.position.x, pos.position.y, room.transform.position.z - 3.5f);
            // Debug.Log("go south: " + room.roomCoords.x + "," + room.roomCoords.y);
        }
    }
}

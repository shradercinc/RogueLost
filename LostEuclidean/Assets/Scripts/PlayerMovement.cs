using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private float StaminaMax = 3;
    private float Stamina;
    private int Health = 3; //set in game manager
    private bool exhausted;
    private float exhaustedT = 0;
    public float WalkSpeed;
    public float RunSpeed;
    private bool running = false;
    private float ZInput = 0;
    private float XInput = 0;

    //animator

    [SerializeField] private Animator roguebanim;
    [SerializeField] private Animator rogueanim;

    private Transform pos;
    private Rigidbody rb;
    public Camera cam;
    private Vector3 mPos;

    private bool Rolling = false;
    public float DashA = 0.05f;
    private Vector3 direct = new Vector3(0, 0, 0);
    public float length = 1;
    private float DTimer = 0;
    //actual cooldown time
    public float coolDownTimer = 0;
    //sets cooldown time at the beginning of the game
    public float coolDown = 1;

    // private float topS = 0; 

    private bool exitStairs = false;

    private float _dripTimer;
    private int _bleeds;
    [SerializeField] private GameObject drip;
    public GameObject staminaBar;
    public GameObject staminaCharge;

    public float healTimer = 0f;

    private float StepT;
    [SerializeField] private float StepRate = 3f;
    [SerializeField] private AudioClip[] StepEffect;
    public AudioSource aud;


    //audio variables for getting hit
    [SerializeField] private AudioClip GetHit;
    [SerializeField] private AudioSource HitAud;
    void Start()
    {
        coolDownTimer = coolDown;
        rb = GetComponent<Rigidbody>();
        pos = GetComponent<Transform>();
        StaminaMax = GameManager.instance.totalStamina;
        Stamina = StaminaMax;
        Health = GameManager.instance.totalHealth;
    }

    /*
    private void OnCollisionStay(Collision other)
    {
        //checks if the object collided was an enemy
        if (other.gameObject.CompareTag("Enemy"))
        {
            //creates enemy var
            var enemy = other.gameObject.GetComponent<Enemy>();
            //checks if the enemy has already been hit
            if (enemy.canHit == true)
            {
                //decreases health, updates healthbar, sets canHit to false redundantly, resets the enemy reload    
                Health--;
                UIManager.instance.UpdateHealthBar(Health);
                enemy.canHit = false;
                enemy.reload = enemy.hitSpeed;
            }
        }
    }
    */
    private void OnTriggerStay(Collider other)
    {
        //checks if the object collided was an enemy
        if (other.gameObject.CompareTag("Enemy"))
        {
            //creates enemy var
            var enemy = other.gameObject.GetComponent<Enemy>();
            //checks if the enemy has already been hit
            if (enemy.canHit == true)
            {
                HitAud.PlayOneShot(GetHit);
                //decreases health, flash blood effect, sets canHit to false redundantly, resets the enemy reload    
                Health--;
                _bleeds = (GameManager.instance.totalHealth - Health) * 10;
                UIManager.instance.UIDamageMessage(Health);
                enemy.canHit = false;
                enemy.reload = enemy.hitSpeed;
            }
        }
    }

    void UpdateStep()
    {
        //setting and reseting the timer between step sound effects
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            StepT -= Time.deltaTime;
        }
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            StepT = 0.2f;
        }
        if (StepT <= 0)
        {
            aud.PlayOneShot(StepEffect[Random.Range(0, StepEffect.Length - 1)]);
            StepT = StepRate / WalkSpeed;
        }
    }

    private void FixedUpdate()
    {
        if (Health < GameManager.instance.totalHealth)
        {
            healTimer += Time.deltaTime;
            // Debug.Log(Health + ", " + healTimer);
            if (healTimer >= 20) //20 seconds to heal 1 health back
            {
                // Debug.Log("HEALED");
                Health++;
                healTimer = 0f;
                if (Health == GameManager.instance.totalHealth)
                {
                    UIManager.instance.UIDamageMessage(Health);
                }
            }
        }

        if (Health <= 0)
        {
            this.gameObject.SetActive(false);
            UIManager.instance.die = true;
            rb.velocity = Vector3.zero;
            // SceneManager.LoadScene(2);
        }
        if (Rolling == false)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            //rb.velocity += transform.forward * XInput * (running == false ? WalkSpeed : RunSpeed);
            //rb.velocity += transform.right * ZInput * (running == false ? WalkSpeed : RunSpeed);
            rb.velocity += transform.right * ZInput * WalkSpeed;
            rb.velocity += transform.forward * XInput * WalkSpeed;
        }

        if (Rolling == true)
        {
            // print("Z =" + direct.z / Mathf.Abs(direct.z));
            // print("X =" + direct.x / Mathf.Abs(direct.x));
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.velocity += transform.forward * XInput * (running == false ? WalkSpeed : RunSpeed);
            rb.velocity += transform.right * ZInput * (running == false ? WalkSpeed : RunSpeed);
            //adds velocity in the direction the player was already moving when dashing. Queries if the velocity is 0 to prevent errors
            rb.velocity += new Vector3(direct.x != 0 ? DashA * Mathf.Sign(direct.x) : 0, 0, direct.z != 0 ? DashA * Mathf.Sign(direct.z) : 0);
        }
        if (rb.velocity != new Vector3(0, 0, 0))
        {
            roguebanim.SetBool("Move", true);
        }
        else
        {
            roguebanim.SetBool("Move", false);
        }

        if (Health == 1 || _bleeds > 0)
        {
            _dripTimer -= Time.deltaTime;
            if (_dripTimer <= 0)
            {
                _dripTimer = 0.5f;
                _bleeds--;
                var temp = Instantiate(drip, new Vector3(transform.position.x, 0.001f, transform.position.z), transform.rotation);
            }
        }
    }
    void Update()
    {
        
        if (!UIManager.instance.start && !UIManager.instance.escape && !UIManager.instance.die)
        {
            UpdateStep();
            if (Rolling == false)
            {
                coolDownTimer -= Time.deltaTime;
                DTimer = 0;
                ZInput = Input.GetAxis("Horizontal");
                XInput = Input.GetAxis("Vertical");
            }
            //start dash
            if (Input.GetKeyDown(KeyCode.Space) && coolDownTimer <= 0)
            {
                Rolling = true;
                direct = rb.velocity;
                coolDownTimer = coolDown;
                UIManager.instance.SetStaminaBarActive();
            }
            if (Rolling == true)
            {
                DTimer += Time.deltaTime;
                if (DTimer >= length)
                {
                    Rolling = false;
                }
            }
            /*
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && exhausted == false)
            {
                running = true;
                Stamina -= Time.deltaTime;
                // UIManager.instance.UpdateStaminaBar(Stamina);
            }
            else
            {
                running = false;
                Stamina += Time.deltaTime;
                // UIManager.instance.UpdateStaminaBar(Stamina);
            }
            */

            if (Stamina <= 0)
            {
                exhausted = true;
                exhaustedT = StaminaMax;
            }
            if (Stamina >= StaminaMax)
            {
                Stamina = StaminaMax;
                // UIManager.instance.UpdateStaminaBar(Stamina);
            }
            if (exhausted == true)
            {
                exhaustedT -= Time.deltaTime;
                if (exhaustedT <= 0)
                {
                    exhausted = false;
                }
            }
            rogueanim.SetFloat("xInput", XInput);
            rogueanim.SetFloat("zInput", ZInput);
            rogueanim.SetBool("run", running);
            //Debug.Log(XInput);
            if (Input.GetKey(KeyCode.A))
            {
                rogueanim.SetBool("Lstrafe", true);
            }
            else
            {
                rogueanim.SetBool("Lstrafe", false);
            }

            if (Input.GetKey(KeyCode.D))
            {
                rogueanim.SetBool("Rstrafe", true);
            }
            else
            {
                rogueanim.SetBool("Rstrafe", false);
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
                // TeleportToCenter(RoomManager.instance.GetStartingRoom());
            }
            else
            {
                //end game
                // SceneManager.LoadScene(1);//end scene
                UIManager.instance.escape = true;
                this.gameObject.SetActive(false);
                rb.velocity = Vector3.zero;
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

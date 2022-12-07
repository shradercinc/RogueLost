using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3;
    private float maxSpeed = 3;
    public float health = 10;
    public int damage = 1;
    public Rigidbody rb;
    public Transform pos;
    private GameObject pl;
    public Coords location;
    public float hx = 0;
    public float hz = 0;
    public float react = 1;
    public float reactm = 1;
    public bool canHit = true;
    public float hitSpeed = 0.5f;
    public float reload = 0;

    //Variables that control enemy slowdown
    [SerializeField] private float accelerate = 0.04f;
    [SerializeField] private float speedDecrease = 2;

    [SerializeField] private GameObject splatter;
    [SerializeField] private GameObject gunSplatter;
    [SerializeField] private GameObject drip;

    private bool _isHurt;
    private float _dripTimer = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        maxSpeed = speed;
        rb = GetComponent<Rigidbody>();
        pos = GetComponent<Transform>();
        pl = GameObject.FindGameObjectWithTag("Player");
        reactm = react;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PistolB"))
        {
            var temp = Instantiate(gunSplatter, new Vector3(transform.position.x, 0.001f, transform.position.z), other.transform.rotation);
            Destroy(other.transform.parent.gameObject);
            health--;
            speed /= speedDecrease;
            _isHurt = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (speed < maxSpeed)
        {
            speed += accelerate;
        }
        else
        {
            speed = maxSpeed;
        }

        //timer when the enemy hits the player
        reload -= Time.deltaTime;
        if (reload <= 0)
        {
            canHit = true;
        }
        else canHit = false;

        //movement towards the player, checks if the reload is done and if the player is in the same room
        if (RoomManager.instance.GetCurrentRoom().roomCoords == location)
        {
            rb.velocity = new Vector3(0, 0, 0);
            if (canHit == true)
            {
                react -= Time.deltaTime;
                //functional movement code
                if (react <= 0)
                {
                    Vector3 dir = (pl.GetComponent<Transform>().position - pos.position);
                    dir.y = 0;
                    dir.Normalize();
                    transform.rotation = Quaternion.LookRotation(dir);
                    rb.velocity += transform.forward * speed;
                    // print("is Moving");
                }
            }
            //reaction timer for when the player first enters the roomX

            if (_isHurt)
            {
                _dripTimer -= Time.deltaTime;
                if (_dripTimer <= 0)
                {
                    _dripTimer = 0.5f;
                    var temp = Instantiate(drip, new Vector3(transform.position.x, 0.001f, transform.position.z), transform.rotation);
                }
            }
        }
        else
        {
            react = reactm;
            Vector3 dir = (new Vector3(hx, pos.position.y, hz) - pos.position);
            dir.y = 0;
            dir.Normalize();
            rb.velocity = new Vector3(0, 0, 0);
            if (Mathf.Sqrt(Mathf.Pow(dir.x, 2) + Mathf.Pow(dir.z, 2)) > 2f)
            {
                transform.rotation = Quaternion.LookRotation(dir);
                rb.velocity += transform.forward * speed;
            }
        }



        if (health <= 0)
        {
            Die();
        }
    }

    // Script for when an enemy dies.
    private void Die()
    {
        var temp = Instantiate(splatter, new Vector3(transform.position.x, 0.001f, transform.position.z), transform.rotation);
        //temp.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Random.ColorHSV();
        Destroy(this.gameObject);
    }
}


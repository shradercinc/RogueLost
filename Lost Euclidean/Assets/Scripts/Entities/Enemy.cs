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

    [SerializeField] private AudioSource HitAud;
    [SerializeField] private AudioClip GetHit;

    private AudioSource aud;
    [SerializeField] private AudioClip[] stepSounds;
    [SerializeField] private float stepRate = 0.35f;
    private float stepTimer = 0.35f;
    private float distVMod = 0.1f;

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
        aud = GetComponent<AudioSource>();
        pos = GetComponent<Transform>();
        pl = GameObject.FindGameObjectWithTag("Player");
        reactm = react;
        stepTimer = 0.1f;
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
        Vector3 dirP = (new Vector3(pl.transform.position.x, pos.position.y, pl.transform.position.z) - pos.position);
        dirP.y = 0;
        var distance = Mathf.Sqrt(Mathf.Pow(dirP.x, 2) + Mathf.Pow(dirP.z, 2));
        aud.volume = 0.6f - (distance * distVMod);

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
                    // print("Distance to player" + distance);
                    // print("volume = " + aud.volume);

                    stepTimer -= Time.deltaTime;
                    if (stepTimer <= 0)
                    {
                        if (speed < maxSpeed)
                        {
                            stepTimer = stepRate * 1.5f;
                        }
                        else stepTimer = stepRate;

                        aud.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Length)]);
                    }
                    Vector3 direction = (pl.GetComponent<Transform>().position - pos.position);
                    direction.y = 0;
                    direction.Normalize();
                    transform.rotation = Quaternion.LookRotation(direction);
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
            Vector3 dir = (new Vector3(hx, pos.position.y, hz) - pos.position);
            dir.y = 0;
            dir.Normalize();
            stepTimer = 0.1f;
            react = reactm;
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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3;
    public float health = 10;
    public int damage = 1;
    public Rigidbody rb;
    public Transform pos;
    private GameObject pl;
    public Coords location; 
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pos = GetComponent<Transform>();
        pl = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PistolB"))
        {
            Destroy(other.transform.parent.gameObject);
            health--;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(RoomManager.instance.GetCurrentRoom().roomCoords == location)
        {
            Vector3 dir = (pl.GetComponent<Transform>().position - pos.position);
            dir.y = 0;
            dir.Normalize();
            transform.rotation = Quaternion.LookRotation(dir);
            rb.velocity = new Vector3(0, 0, 0);
            rb.velocity += transform.forward * speed;
        }


        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}


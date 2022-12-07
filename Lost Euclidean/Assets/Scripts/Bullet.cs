using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    private Vector3 mPos;
    private Transform pos;
    public GameObject pl;
    public Camera cam;
    public Rigidbody rb;
    private float deathT;
    public float accuracy = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        pl = GameObject.FindGameObjectWithTag("Player");
        cam = Camera.main;
        mPos = Input.mousePosition;   
        pos = GetComponent<Transform>();
        var pPos = cam.WorldToScreenPoint(pl.transform.position);
        //Debug.Log(mPos + "|" + pPos + "|" + Input.mousePosition);
        Vector3 dir = (mPos - pPos);
        dir.z = dir.y;
        dir.y = 0f;
        var xacc = Random.Range(-accuracy, accuracy);
        var zacc = Random.Range(-accuracy, accuracy);
        dir.x += xacc;
        dir.z += zacc;
        dir.Normalize();

        transform.rotation = Quaternion.LookRotation(dir);
        rb = GetComponent<Rigidbody>();
        rb.velocity = dir * speed;
        Debug.Log("V=" + rb.velocity);

        deathT = 3;
    }

    // Update is called once per frame
    void Update()
    {
        deathT -= Time.deltaTime;
        if (deathT <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}

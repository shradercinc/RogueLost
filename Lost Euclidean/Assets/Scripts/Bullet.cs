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
    // Start is called before the first frame update
    void Start()
    {
        pl = GameObject.FindGameObjectWithTag("Player");
        cam = Camera.main;
        mPos = Input.mousePosition;   
        pos = GetComponent<Transform>();
        var pPos = cam.WorldToScreenPoint(pl.transform.position);
        Debug.Log(mPos + "|" + pPos + "|" + Input.mousePosition);
        Vector3 dir = (mPos - pPos);
        dir.z = dir.y;
        dir.y = 0f;
        dir.Normalize();
        transform.rotation = Quaternion.LookRotation(dir);
        rb = GetComponent<Rigidbody>();
        rb.velocity = dir * speed;

        deathT = 5;
    }

    // Update is called once per frame
    void Update()
    {
        deathT -= Time.deltaTime;
        if (deathT <= 0)
        {
            Destroy(this);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    private Transform pos;
    public Camera cam;
    private Vector3 mPos;
    [SerializeField] float turnSpeed = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        pos = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        mPos = Input.mousePosition;
        var pPos = cam.WorldToScreenPoint(pos.position);
        Vector3 dir = (mPos - pPos);
        dir.z = dir.y;
        dir.y = 0f;
        dir.Normalize();
        var look = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(pos.rotation, look, turnSpeed);
    }
}

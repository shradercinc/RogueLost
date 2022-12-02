using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firing : MonoBehaviour
{
    private int ammo = 10;
    public float inaccuracy = 15;
    public float fireR = 0.5f;
    public float fireT = 0.5f;
    private Transform pos;
    public GameObject Bullet;
    private Vector3 mPos;
    private Quaternion target;
    [SerializeField] private Animator roguebanim;
    [SerializeField] private Animator rogueanim;

    // Start is called before the first frame update
    void Start()
    {
        fireT = fireR;
        pos = GetComponent<Transform>();
        GameManager.instance.bulletAmount = ammo;
    }

    // Update is called once per frame
    void Update()
    {
        mPos = Input.mousePosition;
        fireT += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Mouse0) && fireT > fireR)
        {
            fireT = 0;
            Object.Instantiate(Bullet, pos.position, pos.rotation);
            //to be entered when ammo is functional or nessecary
            // ammo--;
            UIManager.instance.UpdateAmmo();
            roguebanim.SetBool("Shoot", true);
        }
        else
        {
            
            roguebanim.SetBool("Shoot", false);
        }
    }
}

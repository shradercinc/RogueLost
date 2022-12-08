using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firing : MonoBehaviour
{
    private int ammo = 12;
    public float inaccuracy = 15;
    public float fireR = 0.5f;
    public float fireT = 0.5f;
    private Transform pos;
    public GameObject Bullet;
    public int clipSize = 6;
    public float reloadSpeed = 2;
    private int clip = 6;
    public float reload = 0;
    private Vector3 mPos;
    private Quaternion target;
    [SerializeField] private Animator roguebanim;
    [SerializeField] private Animator rogueanim;
    private bool reloading = false;
    private int leftoverAmmo;

    // Start is called before the first frame update
    void Start()
    {
        fireT = fireR;
        pos = GetComponent<Transform>();
        GameManager.instance.bulletAmount = ammo;
        reload = 0;
        clip = clipSize;
        leftoverAmmo = ammo - clip;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(clip + ", " + leftoverAmmo);
        mPos = Input.mousePosition;
        fireT += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Mouse0) && fireT > fireR && ammo > 0 && clip > 0)
        {
            fireT = 0;
            Object.Instantiate(Bullet, pos.position, pos.rotation);
            //to be entered when ammo is functional or nessecary
            ammo--;
            clip--;
            // UIManager.instance.UpdateAmmo();
            roguebanim.SetBool("Shoot", true);
            UIManager.instance.UpdateAmmo();
            // print(clip + "/" + clipSize);
            // print("Ammo = " + ammo);
        }
        else
        {

            roguebanim.SetBool("Shoot", false);
        }

        reload -= Time.deltaTime;
        if (((Input.GetKeyDown(KeyCode.R)) && clip < clipSize) && reloading == false)
        {
            reloading = true;
            reload = reloadSpeed;
            // UIManager.instance.UpdateAmmo();
            // print("Reloading!");
        }
        if (reloading == true && reload < 0)
        {
            reloading = false;
            if (ammo > clipSize)
                clip = clipSize;
            else
                clip = ammo;

            leftoverAmmo = ammo - clip;
            UIManager.instance.UpdateAmmo();
            // print("Ready to go!");
        }
    }

    //get total ammo left
    public int GetAmmo()
    {
        return ammo;
    }

    //get amount of bullets in clip
    public int GetClip()
    {
        return clip;
    }

    //get leftover ammo 
    public int GetLeftoverAmmo()
    {
        return leftoverAmmo;
    }

}

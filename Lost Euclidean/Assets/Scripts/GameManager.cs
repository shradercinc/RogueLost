using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public static GameManager instance;

    //activated pillars
    public bool bluePillar, yellowPillar, greenPillar, purplePillar;
    // public bool hasGun;
    public bool isTeleporting = true;
    public int bulletAmount = 25;
    public int totalHealth = 5;
    public float totalStamina = 3;

    public SpriteRenderer distort;
    public SpriteRenderer portalDistort;

    private void Awake()
    {
        instance = this;
    }

    public void CreatePlayer()
    {
        Transform player = Instantiate(playerPrefab, RoomManager.instance.GetCurrentRoom().transform).transform;
        distort = player.GetChild(1).GetComponent<SpriteRenderer>();
        portalDistort = GameObject.FindGameObjectWithTag("distort").GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (bluePillar && yellowPillar && greenPillar && purplePillar && isTeleporting)
        {
            distort.gameObject.SetActive(false);
            isTeleporting = false;
            RoomManager.instance.TurnOffAllLights();
        }
    }

    public int TotalFoundPillars()
    {
        return CountTrue(bluePillar, yellowPillar, greenPillar, purplePillar);
    }

    private static int CountTrue(params bool[] args)
    {
        return args.Count(t => t);
    }
}

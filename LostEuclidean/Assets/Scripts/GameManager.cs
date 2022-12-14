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
    [HideInInspector]
    public int totalHealth = 3;
    public float totalStamina = 3;

    public SpriteRenderer distort;
    public SpriteRenderer portalDistort;

    //player reference
    public GameObject playerGO;

    public Color[] colorsPillar;

    private void Awake()
    {
        instance = this;
    }

    public void CreatePlayer()
    {
        Transform player = Instantiate(playerPrefab, RoomManager.instance.GetCurrentRoom().transform).transform;
        distort = player.GetChild(1).GetComponent<SpriteRenderer>();
        portalDistort = GameObject.FindGameObjectWithTag("distort").GetComponent<SpriteRenderer>();
        distort.sharedMaterial.SetColor("_PillarColor0", colorsPillar[0]);
        distort.sharedMaterial.SetColor("_PillarColor1", colorsPillar[1]);
        distort.sharedMaterial.SetColor("_PillarColor2", colorsPillar[2]);
        distort.sharedMaterial.SetColor("_PillarColor3", colorsPillar[3]);
        playerGO = player.gameObject;

        UIManager.instance.playerGO = playerGO;
        UIManager.instance.playerScript = playerGO.GetComponent<PlayerMovement>();
        UIManager.instance.stamina_bar_UI = playerGO.GetComponent<PlayerMovement>().staminaBar;
        UIManager.instance.stamina_charge_UI = playerGO.GetComponent<PlayerMovement>().staminaCharge;
    }

    private void Update()
    {
        if (bluePillar && yellowPillar && greenPillar && purplePillar && isTeleporting)
        {
            distort.gameObject.SetActive(false);
            UIManager.instance.ToggleGlitch(false);
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

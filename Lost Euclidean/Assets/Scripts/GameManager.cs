using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public static GameManager instance;

    //activated pillars
    public bool bluePillar, yellowPillar, greenPillar, purplePillar;
    // public bool hasGun;
    public bool isTeleporting = true;
    public int bulletAmount = 30;

    private void Awake()
    {
        instance = this;
    }

    public void CreatePlayer()
    {
        Instantiate(playerPrefab, RoomManager.instance.GetCurrentRoom().transform);
    }

    private void Update()
    {
        if (bluePillar && yellowPillar && greenPillar && purplePillar && isTeleporting)
        {
            isTeleporting = false;
            RoomManager.instance.TurnOffAllLights();
        }
    }

}

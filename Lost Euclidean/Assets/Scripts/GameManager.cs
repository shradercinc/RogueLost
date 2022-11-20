using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void CreatePlayer()
    {
        Instantiate(playerPrefab, RoomManager.instance.GetCurrentRoom().transform);
    }
}

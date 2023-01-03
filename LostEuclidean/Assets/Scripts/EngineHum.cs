using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineHum : MonoBehaviour
{
    public RoomManager RoomManager;
    private Transform pos;
    private AudioSource aud;
    public GameManager Manager;
    [SerializeField] private float level = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        RoomManager = GameObject.Find("Room Manager").GetComponent<RoomManager>();

        pos = GetComponent<Transform>();
        aud = GetComponent<AudioSource>();
        Manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (aud.volume < level)
        {
            aud.volume += 0.01f;
        }
        if (aud.volume > level)
        {
            aud.volume -= 0.01f;
        }
        var Position = RoomManager.GetCurrentRoom().roomCoords;
        if (Manager.isTeleporting == false)
        {
            level = 0;
        }
        else if (RoomManager.CheckIfHasPillar(Position))
        {
            level = 1f;
        }
        else if (RoomManager.CheckIfHasPillar(Position.x + 1, Position.y)
          || RoomManager.CheckIfHasPillar(Position.x - 1, Position.y)
          || RoomManager.CheckIfHasPillar(Position.x, Position.y + 1)
          || RoomManager.CheckIfHasPillar(Position.x, Position.y - 1))
        {
            level = 0.4f;
        }

        else
        { 
            level = 0.2f;
        }
    }
}

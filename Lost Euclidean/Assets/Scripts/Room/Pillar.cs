using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    private bool canActivate = false;
    private bool activated = false;
    public RoomManager.RoomState state;

    private void Update()
    {
        if (!activated && canActivate && Input.GetKeyDown(KeyCode.E))
        {
            activated = true;
            UIManager.instance.SetInstructionText("Pillar Activated.");
            switch (state)
            {
                case RoomManager.RoomState.blue:
                    GameManager.instance.bluePillar = true;
                    break;
                case RoomManager.RoomState.green:
                    GameManager.instance.greenPillar = true;
                    break;
                case RoomManager.RoomState.purple:
                    GameManager.instance.purplePillar = true;
                    break;
                case RoomManager.RoomState.yellow:
                    GameManager.instance.yellowPillar = true;
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HERE");
        if (other.tag == "Player")
        {
            canActivate = true;
            if (!activated)
            {
                UIManager.instance.SetInstructionText("E to Activate Pillar.");
            }
            else
            {
                UIManager.instance.SetInstructionText("Pillar Activated.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canActivate = false;
            UIManager.instance.HideInstructionText();
        }
    }
}

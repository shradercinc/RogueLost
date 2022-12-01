using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    private bool canActivate = false;
    private bool activated = false;
    public RoomManager.RoomState state;
    [SerializeField] private GameObject distortion;

    private void Update()
    {
        if (!activated && canActivate && Input.GetKeyDown(KeyCode.E))
        {
            activated = true;
            distortion.SetActive(false);
            UIManager.instance.SetInstructionText("Pillar Activated.");
            
            switch (state)
            {
                case RoomManager.RoomState.blue:
                    GameManager.instance.bluePillar = true;
                    GameManager.instance.distort.material.SetColor("_PillarColor3", Color.clear);
                    RoomManager.instance.RegenerateLinks();
                    break;
                case RoomManager.RoomState.green:
                    GameManager.instance.greenPillar = true;
                    GameManager.instance.distort.material.SetColor("_PillarColor0", Color.clear);
                    RoomManager.instance.RegenerateLinks();
                    break;
                case RoomManager.RoomState.purple:
                    GameManager.instance.purplePillar = true;
                    GameManager.instance.distort.material.SetColor("_PillarColor1", Color.clear);
                    RoomManager.instance.RegenerateLinks();;
                    break;
                case RoomManager.RoomState.yellow:
                    GameManager.instance.yellowPillar = true;
                    GameManager.instance.distort.material.SetColor("_PillarColor2", Color.clear);
                    RoomManager.instance.RegenerateLinks();
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HERE");
        if (other.CompareTag("Player"))
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
        if (other.CompareTag("Player"))
        {
            canActivate = false;
            UIManager.instance.HideInstructionText();
        }
    }
}

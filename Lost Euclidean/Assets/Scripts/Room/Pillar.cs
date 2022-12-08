using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    public bool charging = false;
    public bool disabled = false;
    private float timer = 0f;
    private int disableTime = 3;
    // [HideInInspector]
    public int chargeTimer = 0;
    public RoomManager.RoomState state;
    [SerializeField]
    private GameObject distortion;

    [SerializeField] private SpriteRenderer chargeRing;
    private static readonly int Fill = Shader.PropertyToID("_Fill");

    private void Update()
    {
        if (charging && !disabled)
        {
            //increase timer
            timer += Time.deltaTime;
            if (timer % 60 > 1)
            {
                timer = 0;
                chargeTimer++;
                
                UIManager.instance.UIDisablingGeneratorMessage();
            }
            chargeRing.material.SetFloat(Fill, ((float)(chargeTimer) + (timer))/disableTime);
            if (chargeTimer == disableTime)
            {
                UIManager.instance.UIDisablingGeneratorMessage();
                DeactivteGenerator();
            }
        }
    }

    private void DeactivteGenerator()
    {
        disabled = true;
        distortion.SetActive(false);

        switch (state)
        {
            case RoomManager.RoomState.blue:
                GameManager.instance.bluePillar = true;
                GameManager.instance.distort.material.SetColor("_PillarColor3", Color.clear);
                GameManager.instance.portalDistort.material.SetColor("_PillarColor3", Color.clear);
                RoomManager.instance.RegenerateLinks();
                UIManager.instance.UIFoundGeneratorsMessage();
                break;
            case RoomManager.RoomState.green:
                GameManager.instance.greenPillar = true;
                GameManager.instance.distort.material.SetColor("_PillarColor0", Color.clear);
                GameManager.instance.portalDistort.material.SetColor("_PillarColor0", Color.clear);

                RoomManager.instance.RegenerateLinks();
                UIManager.instance.UIFoundGeneratorsMessage();
                break;
            case RoomManager.RoomState.purple:
                GameManager.instance.purplePillar = true;
                GameManager.instance.distort.material.SetColor("_PillarColor1", Color.clear);
                GameManager.instance.portalDistort.material.SetColor("_PillarColor1", Color.clear);

                RoomManager.instance.RegenerateLinks();
                UIManager.instance.UIFoundGeneratorsMessage();
                break;
            case RoomManager.RoomState.yellow:
                GameManager.instance.yellowPillar = true;
                GameManager.instance.distort.material.SetColor("_PillarColor2", Color.clear);
                GameManager.instance.portalDistort.material.SetColor("_PillarColor2", Color.clear);

                RoomManager.instance.RegenerateLinks();
                UIManager.instance.UIFoundGeneratorsMessage();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("HERE");
        if (other.CompareTag("Player"))
        {
            charging = true;
            UIManager.instance.UIDisablingGeneratorMessage();
            UIManager.instance.currentPillar = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            charging = false;
            chargeTimer = 0;
            timer = 0;
            UIManager.instance.UIStopDisablingMessage();
        }
    }

    public string GetCurrentChargePercentage()
    {
        float temp = ((float)chargeTimer / (float)disableTime) * 100;
        return temp.ToString("00");
    }
}

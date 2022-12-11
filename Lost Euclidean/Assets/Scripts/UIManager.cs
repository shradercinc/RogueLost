using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private TextMeshProUGUI chatbox_UI;
    [SerializeField] private GameObject blood_UI;

    public GameObject stamina_bar_UI; //for game manager to set
    public GameObject stamina_charge_UI; //for game manager to set
    [SerializeField] private GameObject minimap_room_UI;
    [SerializeField] private TextMeshProUGUI ammo_UI;
    [SerializeField] private TextMeshProUGUI bigText_UI;

    [SerializeField] private GameObject room_UI_Prefab;
    private ArrayList minimapRooms;

    [SerializeField] private Image glitchEffect;


    [HideInInspector] public GameObject playerGO;
    [HideInInspector] public PlayerMovement playerScript;
    [HideInInspector] public Pillar currentPillar; //set when charging generator

    private bool chargingStamina = false;
    public bool start = true;
    private int textIndex = 0;
    public bool escape = false;
    public bool die = false;


    //all possible messages, do not modify order, only add messages
    private string[] messages =
    {
        "System: Disabling generator: {1}%.",
        "System: {0}/4 generators found.",
        "System: Mission - Disable all generators.", //mission message
        "System: STOP THE ANOMALY.", //mission message
        "System: Major injury received.", //2 health
        "System: Rapidly losing blood.", //1 health
        "System: Vitals stable.", //full health
        "System: Mission failed.", //death
        "System: Generator disabled.",
        "System: Stopped disabling generator."
    };

    private string[] startText =
    {
        "This is the Euclidean Wormhole Research lab.",
        "Something has gone awry and it is you must fix the anomaly to escape.",
        "Your communicator on the bottom left will give you vital information about the mission status as well as your own, use it well.",
        "Turn off the generators that power the anomaly.",
        "Escape through the portal in this room. Good luck.",
    };

    private string[] escapeText =
    {
        "Congradulations on stopping the anomaly.",
        "(Press space to try again.)"
    };

    private string[] dieText =
    {
        "Mission failed...",
        "(Press space to try again.)"
    };


    private string[] chat;

    private Coroutine flashBlood;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //chat
        chat = new string[3];
        chat[0] = messages[2];
        chat[1] = messages[3];
        chat[2] = string.Format(messages[1], GameManager.instance.TotalFoundPillars(), "");
        SetChat();

        blood_UI.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        ammo_UI.text = "6 • 12";

        glitchEffect.material.SetColor("_Color0", GameManager.instance.colorsPillar[0]);
        glitchEffect.material.SetColor("_Color1", GameManager.instance.colorsPillar[1]);
        glitchEffect.material.SetColor("_Color2", GameManager.instance.colorsPillar[2]);
        glitchEffect.material.SetColor("_Color3", GameManager.instance.colorsPillar[3]);

        minimapRooms = new ArrayList();
    }

    private void Update()
    {
        if (start)
        {
            bigText_UI.text = startText[textIndex];
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (textIndex + 1 < startText.Length)
                {
                    textIndex++;
                }
                else
                {
                    start = false;
                    bigText_UI.gameObject.SetActive(false);
                    textIndex = 0;
                }
            }
        }

        if (escape)
        {
            bigText_UI.gameObject.SetActive(true);
            bigText_UI.text = escapeText[textIndex];
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (textIndex + 1 < escapeText.Length)
                {
                    textIndex++;
                }
                else
                {
                    escape = false;
                    SceneManager.LoadScene(0);
                }
            }
        }

        if (die)
        {
            bigText_UI.gameObject.SetActive(true);
            bigText_UI.text = dieText[textIndex];
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (textIndex + 1 < dieText.Length)
                {
                    textIndex++;
                }
                else
                {
                    die = false;
                    SceneManager.LoadScene(0);
                }
            }
        }

        if (chargingStamina)
            UpdateStaminaBar();
    }

    //update chat ui
    private void SetChat()
    {
        chatbox_UI.text = chat[0] + "\n" + chat[1] + "\n" + chat[2];
    }

    //messageIndex: index of new mesesage in 'messages' array
    private void UpdateChatMessage(int messageIndex)
    {
        string str;
        if (currentPillar != null)
            str = string.Format(messages[messageIndex], GameManager.instance.TotalFoundPillars(),
                currentPillar.GetCurrentChargePercentage());
        else
            str = string.Format(messages[messageIndex], GameManager.instance.TotalFoundPillars(), "0");
        chat[0] = chat[1];
        chat[1] = chat[2];
        chat[2] = str;
        SetChat();
    }

    public void UIDamageMessage(int health)
    {
        // Debug.Log(health);
        switch (health)
        {
            case 3:
                UpdateChatMessage(6);
                break;
            case 2:
                UpdateChatMessage(4);
                UIManager.instance.FlashBlood(health);
                break;
            case 1:
                UpdateChatMessage(5);
                UIManager.instance.FlashBlood(health);
                break;
            case 0:
                UpdateChatMessage(7);
                UIManager.instance.FlashBlood(health);
                break;
        }
    }

    public void UIDisablingGeneratorMessage()
    {
        UpdateChatMessage(0);
    }

    public void UIStopDisablingMessage()
    {
        UpdateChatMessage(9);
    }

    public void UIFoundGeneratorsMessage()
    {
        UpdateChatMessage(8);
        UpdateChatMessage(1);
    }

    public void UIMissionMessage()
    {
        UpdateChatMessage(2);
        UpdateChatMessage(3);
    }

    //flash UI blood effect
    private void FlashBlood(int health)
    {
        if (flashBlood != null)
        {
            StopCoroutine(flashBlood);
        }

        if (health != 1) //flash doen't happen if 1 hp
        {
            blood_UI.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            flashBlood = StartCoroutine(FadeImage(blood_UI.GetComponent<Image>()));
        }
    }

    //blood fade
    private IEnumerator FadeImage(Image img)
    {
        // fade from opaque to transparent
        // loop over 1 second backwards
        for (float i = 1; i >= 0; i -= Time.deltaTime / 4)
        {
            // set color with i as alpha
            img.color = new Color(1, 1, 1, i);
            yield return null;
        }
    }

    public void SetStaminaBarActive()
    {
        stamina_bar_UI.SetActive(true);
        chargingStamina = true;
    }

    public void UpdateStaminaBar()
    {
        float percentage = (float)1f - playerScript.coolDownTimer / 1;
        // Debug.Log("p: " + percentage);
        if (percentage < 1)
        {
            var color = stamina_charge_UI.GetComponent<SpriteRenderer>().color;
            color.a = 0.3f;
            stamina_charge_UI.GetComponent<SpriteRenderer>().color = color;
            stamina_charge_UI.transform.localScale = new Vector3(0.8f, percentage * 0.7f, 1f);
        }
        else if (percentage >= 1)
        {
            chargingStamina = false;
            stamina_bar_UI.SetActive(false);
        }
    }

    public void UpdateMinimap()
    {
        //clear old rooms
        foreach (GameObject room in minimapRooms)
        {
            Destroy(room);
        }

        //offset (x: 87, y: 42)
        Coords currentCoords = RoomManager.instance.GetCurrentRoom().roomCoords;
        foreach ((int, int) coord in RoomManager.instance.foundRooms.Keys)
        {
            var roomMapIcon = Instantiate(room_UI_Prefab, minimap_room_UI.transform);
            roomMapIcon.GetComponent<RectTransform>().anchoredPosition3D =
                new Vector3(0f + coord.Item1 * 87f, 0f + coord.Item2 * 42f, 0f);
            minimapRooms.Add(roomMapIcon);
            UI_MinimapRoom roomDetail = roomMapIcon.GetComponent<UI_MinimapRoom>();
            if (RoomManager.instance.foundRooms[coord].nsewExits.north)
            {
                roomDetail.topWall.SetActive(false);
            }

            if (RoomManager.instance.foundRooms[coord].nsewExits.south)
            {
                roomDetail.botWall.SetActive(false);
            }

            if (RoomManager.instance.foundRooms[coord].nsewExits.east)
            {
                roomDetail.rightWall.SetActive(false);
            }

            if (RoomManager.instance.foundRooms[coord].nsewExits.west)
            {
                roomDetail.leftWall.SetActive(false);
            }

            if (RoomManager.instance.CheckIfHasPillar(coord.Item1, coord.Item2))
            {
                roomDetail.generator.SetActive(true);
            }
        }

        minimap_room_UI.GetComponent<RectTransform>().anchoredPosition3D =
            new Vector3(0f - currentCoords.x * 87f, 0f - currentCoords.y * 42f, 0f);
    }

    public void UpdateAmmo()
    {
        var str = playerGO.GetComponent<Firing>().GetClip() + " • " + playerGO.GetComponent<Firing>().GetLeftoverAmmo();
        ammo_UI.text = str;
    }

    public void ToggleGlitch(bool active)
    {
        glitchEffect.gameObject.SetActive(active);
    }

}
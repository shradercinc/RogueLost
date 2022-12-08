using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System.Text.RegularExpressions;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private TextMeshProUGUI chatbox_UI;
    [SerializeField]
    private GameObject blood_UI;
    [SerializeField]
    private GameObject stamina_UI;
    [SerializeField]
    private GameObject minimap_room_UI;
    [SerializeField]
    private TextMeshProUGUI ammo_UI;

    [SerializeField]
    private GameObject room_UI_Prefab;
    private ArrayList minimapRooms;


    [HideInInspector]
    public GameObject playerGO;
    [HideInInspector]
    public PlayerMovement playerScript;


    //all possible messages, do not modify order, only add messages
    private string[] messages = {
        "Disabling generator: {1}%.",
        "{0}/4 generators found.",
        "Mission: Disable all generators.", //mission message
        "STOP THE ANOMOLY.", //mission message
        "Minor injury recieved.", //4 health
        "Significant injury recieved.", //3 health
        "Rapidly losing blood.", //2 health
        "Vitals critical, SEEK AID.", //1 health
        "Mission failed.", //death
        "Generator disabled."
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
        chat[2] = string.Format(messages[1], GameManager.instance.TotalFoundPillars(), "_N/A_");
        SetChat();

        blood_UI.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        ammo_UI.text = "6 • 12";

        minimapRooms = new ArrayList();
    }

    private void Update()
    {
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
        string str = string.Format(messages[messageIndex], GameManager.instance.TotalFoundPillars(), "_N/A_"); //TODO: get number of generators, percentage complete
        chat[0] = chat[1];
        chat[1] = chat[2];
        chat[2] = str;
        SetChat();
    }

    public void UIDamageMessage(int Health)
    {
        switch (Health)
        {
            case 4:
                UpdateChatMessage(4);
                break;
            case 3:
                UpdateChatMessage(5);
                break;
            case 2:
                UpdateChatMessage(6);
                break;
            case 1:
                UpdateChatMessage(7);
                break;
            case 0:
                UpdateChatMessage(8);
                break;
        }
    }

    public void UIDisablingGeneratorMessage()
    {
        UpdateChatMessage(0);
    }

    public void UIFoundGeneratorsMessage()
    {
        UpdateChatMessage(9);
        UpdateChatMessage(1);
    }

    public void UIMissionMessage()
    {
        UpdateChatMessage(2);
        UpdateChatMessage(3);
    }

    //flash UI blood effect
    public void FlashBlood()
    {
        if (flashBlood != null)
        {
            StopCoroutine(flashBlood);
        }
        blood_UI.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        flashBlood = StartCoroutine(FadeImage(blood_UI.GetComponent<Image>()));
    }

    //blood fade
    private IEnumerator FadeImage(Image img)
    {
        // fade from opaque to transparent
        // loop over 1 second backwards
        for (float i = 1; i >= 0; i -= Time.deltaTime / 4)
        {
            // Debug.Log(i);
            // set color with i as alpha
            img.color = new Color(1, 1, 1, i);
            yield return null;
        }
    }

    public void UpdateStaminaBar()
    {
        float percentage = (float)1f - playerScript.coolDownTimer / 1;
        // Debug.Log("p: " + percentage);
        if (percentage < 1)
        {
            var color = stamina_UI.GetComponent<Image>().color;
            color.a = 0.3f;
            stamina_UI.GetComponent<Image>().color = color;
            stamina_UI.transform.localScale = new Vector3(1f, percentage * 1f, 1f);
        }
        else if (percentage >= 1)
        {
            var color = stamina_UI.GetComponent<Image>().color;
            color.a = 1f;
            stamina_UI.GetComponent<Image>().color = color;
            stamina_UI.transform.localScale = new Vector3(1f, 1f, 1f);
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
            roomMapIcon.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f + coord.Item1 * 87f, 0f + coord.Item2 * 42f, 0f);
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
        minimap_room_UI.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f - currentCoords.x * 87f, 0f - currentCoords.y * 42f, 0f);
    }

    public void UpdateAmmo()
    {
        var str = playerGO.GetComponent<Firing>().GetClip() + " • " + playerGO.GetComponent<Firing>().GetLeftoverAmmo();
        ammo_UI.text = str;
    }
}
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
    private GameObject minimap_UI;
    [SerializeField]
    private GameObject ammo_UI;

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
        //minimap
        // startLoc.x = playerIcon.GetComponent<RectTransform>().position.x;
        // startLoc.y = playerIcon.GetComponent<RectTransform>().position.y;

        // //ammo
        // ammoStack = new Stack<GameObject>();
        // GenerateAmmo();
        // HideInstructionText();

        //chat
        chat = new string[3];
        chat[0] = messages[2];
        chat[1] = messages[3];
        chat[2] = messages[1];
        SetChat();

        blood_UI.GetComponent<Image>().color = new Color(1, 1, 1, 0);
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
        //go to 0
        //lower opacity as it increases
        //when refilled, full opacity
    }

    public void UpdateMinimap()
    {
        //change position of map
        //load in room if new and not currently loaded
        //draw in blocked exits
        //draw relavent information
    }

    public void GenerateAmmo()
    {

    }

    public void UpdateAmmo()
    {

    }


    // public void SetInstructionText(string str)
    // {
    //     instructionText.GetComponent<TextMeshProUGUI>().text = str;
    //     instructionText.SetActive(true);
    // }

    // public void HideInstructionText()
    // {
    //     instructionText.SetActive(false);
    // }

    // public void GenerateAmmo()
    // {
    //     for (int i = 0; i < GameManager.instance.bulletAmount; i++)
    //     {
    //         float offset = 0f;
    //         if (i / 5 > 0)
    //         {
    //             offset = (i / 5) * 5f;
    //         }

    //         GameObject ammo = Instantiate(ammoPrefab, ammoUI.transform);
    //         Vector3 pain = new Vector3(ammoUI.GetComponent<RectTransform>().transform.position.x, ammoUI.GetComponent<RectTransform>().transform.position.y - (15f * i) - offset, ammoUI.GetComponent<RectTransform>().transform.position.z);
    //         //            Debug.Log(pain);
    //         ammo.GetComponent<RectTransform>().position = pain;
    //         ammoStack.Push(ammo);
    //     }
    // }

    // public void UpdateAmmo()
    // {
    //     GameObject ammo = ammoStack.Pop();
    //     Destroy(ammo);
    // }

    // //call to change player location on minimap
    // public void UpdateMinimap(Coords coords)
    // {
    //     playerIcon.GetComponent<RectTransform>().position = new Vector3(startLoc.x + (48 * coords.x), startLoc.y + (48 * coords.y), 0f);
    //     //playerIcon.GetComponent<RectTransform>().position = new Vector3(startLoc.x + (66 * coords.x), startLoc.y + (66 * coords.y), 0f);
    // }

    // public void UpdateHealthBar(int newHealth)
    // {
    //    // Debug.Log(newHealth);
    //     //lower healthbar
    //     float percentage = (float)newHealth / GameManager.instance.totalHealth;
    //     Debug.Log("p: "+percentage);
    //     if (percentage <= 0)
    //     {
    //         healthBar.SetActive(false);
    //     }
    //     else
    //     {
    //         healthBar.transform.localScale = new Vector3(percentage * 1f, 1f, 1f);
    //     }
    // }

    // public void UpdateStaminaBar(float newStamina)
    // {
    //     float percentage = (float)newStamina / GameManager.instance.totalStamina;
    //     //Debug.Log("p: "+percentage);
    //     if (percentage <= 0)
    //     {
    //         staminaBar.SetActive(false);
    //     }
    //     else
    //     {
    //         staminaBar.transform.localScale = new Vector3(percentage * 1f, 1f, 1f);
    //     }
    // }
}
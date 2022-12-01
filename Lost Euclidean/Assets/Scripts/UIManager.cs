using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject instructionText;

    //minimap
    [SerializeField]
    private GameObject playerIcon;
    //starting point of player on map
    private (float x, float y) startLoc;

    //ammo indicator
    [SerializeField]
    private GameObject ammoUI;
    [SerializeField]
    private GameObject ammoPrefab;
    private Stack<GameObject> ammoStack;


    //health bar
    [SerializeField]
    private GameObject healthBar;
    private Vector3 healthLocalScale;
    public int healthScale;

    //stamina bar
    [SerializeField]
    private GameObject staminaBar;
    private Vector3 staminaLocalScale;

    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //minimap
        startLoc.x = playerIcon.GetComponent<RectTransform>().position.x;
        startLoc.y = playerIcon.GetComponent<RectTransform>().position.y;

        //ammo
        ammoStack = new Stack<GameObject>();
        GenerateAmmo();

        //resource bars
        healthLocalScale = healthBar.transform.localScale;
        staminaLocalScale = staminaBar.transform.localScale;

        HideInstructionText();
    }

    public void SetInstructionText(string str)
    {
        instructionText.GetComponent<TextMeshProUGUI>().text = str;
        instructionText.SetActive(true);
    }

    public void HideInstructionText()
    {
        instructionText.SetActive(false);
    }

    public void GenerateAmmo()
    {
        for (int i = 0; i < GameManager.instance.bulletAmount; i++)
        {
            float offset = 0f;
            if (i / 5 > 0)
            {
                offset = (i / 5) * 5f;
            }

            GameObject ammo = Instantiate(ammoPrefab, ammoUI.transform);
            Vector3 pain = new Vector3(ammoUI.GetComponent<RectTransform>().transform.position.x, ammoUI.GetComponent<RectTransform>().transform.position.y - (15f * i) - offset, ammoUI.GetComponent<RectTransform>().transform.position.z);
            Debug.Log(pain);
            ammo.GetComponent<RectTransform>().position = pain;
            ammoStack.Push(ammo);
        }
    }

    public void UpdateAmmo()
    {
        GameObject ammo = ammoStack.Pop();
        Destroy(ammo);
    }

    //call to change player location on minimap
    public void UpdateMinimap(Coords coords)
    {
        playerIcon.GetComponent<RectTransform>().position = new Vector3(startLoc.x + (68 * coords.x), startLoc.y - (68 * coords.y), 0f);
    }

    public void LowerHealth(int damage)
    {
        //lower healthbar
        // float percentage = (float)currHealth / totalHealth;
        // if (percentage <= 0)
        // {
        //     gameObject.SetActive(false);
        // }
        // else
        // {
        //     healthBar.transform.localScale = new Vector3(percentage * healthScale, 1f, localScale.z);
        // }
    }

    public void LowerStamina(int damage)
    {
    }
}
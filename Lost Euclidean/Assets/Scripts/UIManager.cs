using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject instructionText;
    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
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
}

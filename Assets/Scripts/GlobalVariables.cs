using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GlobalVariables : MonoBehaviour
{
    public GameObject bubbleBox;
    public TextMeshProUGUI bubbleText;
    public GameObject endScreen;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}

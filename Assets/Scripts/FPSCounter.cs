using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    private int avgFrameRate;
    [Header("Drag and drop prefab into your scene")]
    [SerializeField] private TMP_Text display_Text;
    private void Start()
    {
        //Update once every second
        InvokeRepeating(nameof(FPS), 1, 1);
    }
    void FPS()
    {
        avgFrameRate = (int)(1f / Time.unscaledDeltaTime);
        display_Text.text = avgFrameRate + " FPS";
    }
}

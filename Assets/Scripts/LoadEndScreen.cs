using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadEndScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textGUI;
    [Header("0 = None, 1 = Some, 2 = All")]
    [SerializeField] private string[] completionTexts;
    void Start()
    {
        int cubCount = PlayerPrefs.GetInt("cubsSaved");

        if (cubCount == 0)
        {
            //no cubs saved
            Debug.Log("no cubs saved");
            textGUI.text = "You saved " + cubCount + " cubs " + completionTexts[0];
        }
        else if (cubCount > 0 && cubCount < 3)
        {
            //some cubs saved, but not all
            Debug.Log("some cubs saved");
            textGUI.text = "You saved " + cubCount + " cubs " + completionTexts[1];
        }
        else if (cubCount == 3)
        {
            //all cubs saved
            Debug.Log("all cubs saved");
            textGUI.text = "You saved " + cubCount + " cubs " + completionTexts[2];
        }
    }
    public void Restart()
    {
        SceneManager.LoadScene("Main");
    }
    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

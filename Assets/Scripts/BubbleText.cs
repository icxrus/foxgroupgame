using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BubbleText : MonoBehaviour
{
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private float lineDelay = 2f;
    public string[] lines;

    private GlobalVariables globalVariables;
    private GameObject textBox;
    private TextMeshProUGUI textGUI;
    private int index;
    private bool isRunning = false;
    private void Start()
    {
        globalVariables = GameObject.Find("Player").GetComponent<GlobalVariables>();
        textBox = globalVariables.bubbleBox;
        textGUI = globalVariables.bubbleText;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && isRunning == false)
        {
            isRunning = true;
            StartBubble();
        }
    }
    public void StartBubble()
    {
        textGUI.gameObject.SetActive(true);
        textBox.SetActive(true);

        textGUI.text = string.Empty;
        index = 0;
        StartCoroutine(TypeLine());
    }
    IEnumerator TypeLine()
    {
        //write each letter after a short pause
        foreach (char c in lines[index].ToCharArray())
        {
            textGUI.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        yield return new WaitForSeconds(lineDelay);
        NextLine();
    }
    void NextLine()
    {
        Debug.Log("index: " + index + " && " + "lines.length: " + lines.Length);
        //if there are more lines left, count down 1 line and continue the text typing
        if (index < lines.Length - 1)
        {
            index++;
            textGUI.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            textGUI.text = string.Empty;
            index = 0;
            textGUI.gameObject.SetActive(false);
            textBox.SetActive(false);
            isRunning = false;
        }
    }
}

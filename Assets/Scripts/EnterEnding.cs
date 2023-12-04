using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterEnding : MonoBehaviour
{
    private GlobalVariables global;
    private CubDataHolder cubData;
    private GameObject endBox;
    private void Start()
    {
        global = GameObject.Find("Player").GetComponent<GlobalVariables>();
        cubData = GameObject.Find("Puzzle Manager").GetComponent<CubDataHolder>();
        endBox = global.endScreen;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            endBox.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            endBox.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void EndGame()
    {
        int cubCount = cubData.cubsSaved;
        PlayerPrefs.SetInt("cubsSaved", cubCount);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Ending");
    }
    public void KeepPlaying()
    {
        //Debug.Log("yes");
        endBox.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
}

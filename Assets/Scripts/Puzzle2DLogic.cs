using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Puzzle2DLogic : MonoBehaviour
{
    [SerializeField] private bool[] firstClueFound = new bool[2];
    [SerializeField] private bool[] secondClueFound = new bool[3];
    [SerializeField] private bool[] fakeClueFound = new bool[2];

    public GameObject canvas;
    public GameObject fakeCanvas;

    public Image furPile;
    public Image footSteps;
    public Image fakeFurPile;

    public TMP_Text supportText;
    public TMP_Text supportFakeText;

    [SerializeField] private bool clue1Status;
    [SerializeField] private bool clue2Status;
    [SerializeField] private bool clue3Status;

    public CubCollect cubCollect;
    public CubDataHolder cubData;

    public bool puzzle2DCompleted = false;
    public bool inView = false;

    private Transform nextRoom;
    private Transform tpOut;

    private GameObject player;

    private void Awake()
    {
        cubData = gameObject.GetComponent<CubDataHolder>();

        nextRoom = GameObject.FindGameObjectWithTag("TP2D_NextRoom").transform;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void AdjustFirstClue(int i)
    {
        if (!firstClueFound[i])
        {
            firstClueFound[i] = true;
            supportText.enabled = true;
        }

        clue1Status = CheckIfClueIsFound(firstClueFound);
        Debug.Log("Clue 1 Status: " + clue1Status);

        if (clue1Status)
        {
            supportText.enabled = false;
            furPile.enabled = true;
            Debug.Log("Clue 1 visible.");
        }
    }

    public void AdjustSecondClue(int i)
    {
        if (!secondClueFound[i])
        {
            secondClueFound[i] = true;
            supportText.enabled = true;
        }

        clue2Status = CheckIfClueIsFound(secondClueFound);
        Debug.Log("Clue 2 Status: " + clue2Status);

        if (clue2Status)
        {
            supportText.enabled = false;
            footSteps.enabled = true;
            Debug.Log("Clue 2 visible.");
        }
    }

    public void AdjustFakeClue(int i)
    {
        if (!fakeClueFound[i])
        {
            fakeClueFound[i] = true;
            supportFakeText.enabled = true;
        }

        clue3Status = CheckIfClueIsFound(fakeClueFound);
        Debug.Log("Clue 3 Status: " + clue3Status);

        if (clue3Status)
        {
            supportFakeText.enabled = false;
            fakeFurPile.enabled = true;
            Debug.Log("Clue 3 visible.");
        }
    }

    public bool CheckIfClueIsFound(bool[] array)
    {
        bool completed = true;
        foreach (bool checks in array)
        {
            if (!checks)
            {
                completed = false;
            }
        }

        if (completed)
        {
            return true;
        }
        else
            return false;
    }

    public void ExitView()
    {
        canvas.SetActive(false);
        player.SetActive(true);
        inView = false;
        Cursor.visible = true;
    }
    public void ExitFakeView()
    {
        fakeCanvas.SetActive(false);
        player.SetActive(true);
        inView = false;
        Cursor.visible = true;
    }

    public void EnterCorrectWay()
    {
        Debug.Log("Entered correct way.");

        canvas.SetActive(false);
        puzzle2DCompleted = true;
        player.SetActive(true);
        GoToNext();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EnterFakeWay()
    {
        Debug.Log("Entered wrong way.");

        cubData.CubDeath(0);

        fakeCanvas.SetActive(false);
        puzzle2DCompleted = true;
        player.SetActive(true);
        CharacterController charControl = player.GetComponent<CharacterController>();
        charControl.enabled = false;
        player.transform.position = tpOut.position;
        charControl.enabled = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void GoToNext()
    {
        CharacterController charControl = player.GetComponent<CharacterController>();
        charControl.enabled = false;
        player.transform.position = nextRoom.position;
        charControl.enabled = true;
    }
}

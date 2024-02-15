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

    public GameObject canvas2DUI;
    public GameObject fakeCanvas2DUI;

    public Image furPileImage;
    public Image footStepsImage;
    public Image fakeFurPileImage;

    public TMP_Text supportText;
    public TMP_Text supportFakeText;

    [SerializeField] private bool clue1Status;
    [SerializeField] private bool clue2Status;
    [SerializeField] private bool clue3Status;

    public CubCollect cubCollect;
    public CubDataHolder cubDataHolder;

    public bool isPuzzle2DCompleted = false;
    public bool isInsideUIView = false;

    private Transform nextRoomTPLocation;
    private readonly Transform tpOutLocation;

    private GameObject player;

    private void Awake()
    {
        cubDataHolder = gameObject.GetComponent<CubDataHolder>();

        nextRoomTPLocation = GameObject.FindGameObjectWithTag("TP2D_NextRoom").transform;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public int FetchIndexFromButtonsAndReturnIt(int index)
    {
        return index;
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
            furPileImage.enabled = true;
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
            footStepsImage.enabled = true;
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
            fakeFurPileImage.enabled = true;
            Debug.Log("Clue 3 visible.");
        }
    }

    public bool CheckIfClueIsFound(bool[] clueStatusArray)
    {
        bool isClueCompleted = true;
        foreach (bool isFound in clueStatusArray)
        {
            if (!isFound)
            {
                isClueCompleted = false;
            }
        }

        if (isClueCompleted)
        {
            return true;
        }
        else
            return false;
    }

    public void EnterCorrectWay()
    {
        Debug.Log("Entered correct way.");

        canvas2DUI.SetActive(false);
        isPuzzle2DCompleted = true;
        player.SetActive(true);

        GoToNextRoom();

        ReturnCursorStateToPlay();
    }

    public void EnterFakeWay()
    {
        Debug.Log("Entered wrong way.");

        MarkCubDead();

        fakeCanvas2DUI.SetActive(false);
        isPuzzle2DCompleted = true;
        player.SetActive(true);

        //Teleport Player out of puzzle
        CharacterController charControl = player.GetComponent<CharacterController>();
        charControl.enabled = false;
        player.transform.position = tpOutLocation.position;
        charControl.enabled = true;

        ReturnCursorStateToPlay();
    }
    void GoToNextRoom()
    {
        CharacterController charControl = player.GetComponent<CharacterController>();
        charControl.enabled = false;
        player.transform.position = nextRoomTPLocation.position;
        charControl.enabled = true;
    }

    private void MarkCubDead()
    {
        for (int i = 0; i < cubDataHolder.cubData.Count; i++)
        {
            if (cubDataHolder.cubData[i].tagName == "2DPuzzle")
            {
                cubDataHolder.MarkCubDead(cubDataHolder.cubData[i]);
                break;
            }
        }
    }

    private void ReturnCursorStateToPlay()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
}

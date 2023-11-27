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

    public Canvas canvas;
    public Canvas fakeCanvas;

    public Image furPile;
    public Image footSteps;
    public Image fakeFurPile;

    public TMP_Text supportText;
    public TMP_Text supportFakeText;

    [SerializeField] private bool clue1Status;
    [SerializeField] private bool clue2Status;
    [SerializeField] private bool clue3Status;

    public CubCollect cubManager;
    public CubDataHolder cubData;

    public bool puzzle2DCompleted = false;

    private void Awake()
    {
        cubData = gameObject.GetComponent<CubDataHolder>();
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

    public void OpenAreaCanvas()
    {
        canvas.enabled = true;

    }

    public void OpenFakeAreaCanvas()
    {
        fakeCanvas.enabled = true;
    }

    public void ExitView()
    {
        canvas.enabled = false;
    }
    public void ExitFakeView()
    {
        fakeCanvas.enabled = false;
    }

    public void EnterCorrectWay()
    {
        Debug.Log("Entered correct way.");
        canvas.enabled = false;
        cubManager.AddCubFromScript();
        puzzle2DCompleted = true;
        Debug.Log("Completed cub setup.");
    }

    public void EnterFakeWay()
    {
        Debug.Log("Entered wrong way.");
        canvas.enabled = false;
        cubData.CubDeathUpdate(0);
        puzzle2DCompleted = true;
        Debug.Log("Completed cub death update.");
    }
}

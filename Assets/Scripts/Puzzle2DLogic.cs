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

    public void AdjustFirstClue(int i)
    {
        if (!firstClueFound[i])
        {
            firstClueFound[i] = true;
            supportText.enabled = true;
        }

        clue1Status = CheckIfClueIsFound(firstClueFound);

        if (clue1Status)
        {
            supportText.enabled = false;
            furPile.enabled = true;
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

        if (clue2Status)
        {
            supportText.enabled = false;
            footSteps.enabled = true;
        }
    }

    public void AdjustFakeClue(int i)
    {
        if (!fakeClueFound[i])
        {
            fakeClueFound[i] = true;
            supportFakeText.enabled = true;
        }

        clue3Status = CheckIfClueIsFound(firstClueFound);

        if (clue3Status)
        {
            supportFakeText.enabled = false;
            fakeFurPile.enabled = true;
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

    public void EnterArea()
    {
        canvas.enabled = true;
    }

    public void EnterFakeArea()
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
}

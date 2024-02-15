using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenView : MonoBehaviour
{
    public GameObject canvas2DPuzzleUI;
    public GameObject fakeCanvas2DPuzzleUI;
    private GameObject player;
    private Puzzle2DLogic puzzle2DLogic;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        puzzle2DLogic = GameObject.FindGameObjectWithTag("PuzzleManager").GetComponent<Puzzle2DLogic>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!puzzle2DLogic.isInsideUIView)
        {
            if (other.CompareTag("Player") && gameObject.CompareTag("CaveExitA"))
            {
                canvas2DPuzzleUI.SetActive(true);
                player.SetActive(false);
                puzzle2DLogic.isInsideUIView = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (other.CompareTag("Player") && gameObject.CompareTag("CaveExitB"))
            {
                fakeCanvas2DPuzzleUI.SetActive(true);
                puzzle2DLogic.isInsideUIView = true;
                player.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}

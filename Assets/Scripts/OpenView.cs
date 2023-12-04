using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenView : MonoBehaviour
{
    public GameObject canvas;
    public GameObject fakeCanvas;
    private GameObject player;
    private Puzzle2DLogic logic;

    private void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas2DReal");
        fakeCanvas = GameObject.FindGameObjectWithTag("Canvas2DFake");
        player = GameObject.FindGameObjectWithTag("Player");
        logic = GameObject.FindGameObjectWithTag("PuzzleManager").GetComponent<Puzzle2DLogic>();
    }
    private void Start()
    {
        canvas.SetActive(false);
        fakeCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!logic.inView)
        {
            if (other.CompareTag("Player") && gameObject.CompareTag("CaveExitA"))
            {
                canvas.SetActive(true);
                player.SetActive(false);
                logic.inView = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (other.CompareTag("Player") && gameObject.CompareTag("CaveExitB"))
            {
                fakeCanvas.SetActive(true);
                logic.inView = true;
                player.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}

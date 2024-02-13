using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCubLocation : MonoBehaviour
{
    [SerializeField] private CubDataHolder cubDataHolder;
    private void Start()
    {
        cubDataHolder = GameObject.FindGameObjectWithTag("PuzzleManager").GetComponent<CubDataHolder>();

        Invoke("AssignCubToPuzzle", 0.5f);
    }

    void AssignCubToPuzzle()
    {
        foreach (CubDataHolder.CubData cubData in cubDataHolder.cubData)
        {
            Transform here = this.gameObject.transform.parent;
            Transform cubChild = this.gameObject.transform.GetChild(0);

            if (here.CompareTag(cubData.tagName))
            {
                cubData.cubAtPuzzle = cubChild;
            }
        }
    }
    private void Update()
    {
        //check what data was collected
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (CubDataHolder.CubData puzzleData in cubDataHolder.cubData)
            {
                Debug.Log("TagName: " + puzzleData.tagName + ", PuzzleSpawn at: " + puzzleData.puzzleSpawn.position + " CubAtPuzzle at: " + puzzleData.cubAtPuzzle.position);
            }
        }
    }
}

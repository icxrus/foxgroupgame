using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCubLocation : MonoBehaviour
{
    [SerializeField] private CubDataHolder cubData;
    private void Start()
    {
        cubData = GameObject.FindGameObjectWithTag("PuzzleManager").GetComponent<CubDataHolder>();

        Transform here = this.gameObject.transform.parent.parent;
        if (here.CompareTag("2DPuzzle"))
        {
            cubData.cubLocations[0] = this.gameObject;
        }
        else if (here.CompareTag("HiddenPuzzle"))
        {
            cubData.cubLocations[1] = this.gameObject;
        }
        else if (here.CompareTag("ParkourPuzzle"))
        {
            cubData.cubLocations[2] = this.gameObject;
        }
    }
}

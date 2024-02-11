using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCubLocation : MonoBehaviour
{
    [SerializeField] private CubDataHolder cubData;
    private void Start()
    {
        cubData = GameObject.FindGameObjectWithTag("PuzzleManager").GetComponent<CubDataHolder>();

        Transform here = this.gameObject.transform.parent;
        if (here.CompareTag("2DPuzzle"))
        {
            cubData.cubSpawnLocations[0] = this.gameObject;
        }
        else if (here.CompareTag("HiddenPuzzle"))
        {
            cubData.cubSpawnLocations[1] = this.gameObject;
        }
        else if (here.CompareTag("ParkourPuzzle"))
        {
            cubData.cubSpawnLocations[2] = this.gameObject;
        }
    }
}

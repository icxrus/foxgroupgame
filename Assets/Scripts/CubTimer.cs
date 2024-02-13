using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubTimer : MonoBehaviour
{
    [SerializeField] private float timerDuration = 900.0f; // 15 mins in secs
    [SerializeField] private float currentTime;

    private CubDataHolder cubData;
    private bool runOnce = false;
    void Start()
    {
        currentTime = timerDuration;
        cubData = GetComponent<CubDataHolder>();
    }
    void Update()
    {
        RunTimer();
    }
    void RunTimer()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else if (runOnce == false && cubData.cubData.Count > 0)
        {
            // timer reached 0
            runOnce = true;
            currentTime = timerDuration;
            KillRandomCub();
            Debug.Log("Cub dies now");
        }
    }
    void KillRandomCub()
    {
        Debug.Log("KillRandomCub ran");
        int cubToKill = Random.Range(0, cubData.cubData.Count);

            if (cubData.cubData[cubToKill].cubAtPuzzle != null)
            {
                cubData.MarkCubDead(cubData.cubData[cubToKill]);
                runOnce = false;
            }
            else
            {
                //if something fucked up and it gave a null object, try again
                KillRandomCub();
            }
    }
}

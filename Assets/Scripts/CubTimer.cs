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
        //Debug.Log("allCubs is " + allCubs.Count);
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else if (runOnce == false)
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
        int cubToKill = Random.Range(0, cubData.allCubs.Count);

        if (cubData.allCubs[cubToKill].gameObject != null)
        {
            cubData.MarkCubDead(cubToKill);
            runOnce = false;
        }
        else
        {
            //if something fucked up and it gave a null object, try again
            KillRandomCub();
        }
    }
}

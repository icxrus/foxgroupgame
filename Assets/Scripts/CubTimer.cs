using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubTimer : MonoBehaviour
{
    [SerializeField] private float timerDuration = 900.0f; // 15 mins in secs
    [SerializeField] private GameObject deadCubPrefab;
    private List<GameObject> allCubs = new List<GameObject>();
    private float currentTime;
    private bool runOnce = false;

    void Start()
    {
        currentTime = timerDuration;

        GameObject[] taggedCubs = GameObject.FindGameObjectsWithTag("Cub");

        foreach (GameObject cub in taggedCubs)
        {
            allCubs.Add(cub);
        }
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else if (runOnce == false)
        {
            // timer reached 0
            runOnce = true;
            currentTime = timerDuration;
            KillCub();
            Debug.Log("Cub dies now");
        }

        //float minutes = Mathf.Floor(currentTime / 60);
        //float seconds = Mathf.Floor(currentTime % 60);
        //Debug.Log(string.Format("{0:00}:{1:00}", minutes, seconds));
    }
    void KillCub()
    {
        Debug.Log("KillCub ran");
        int cubToKill = Random.Range(0, allCubs.Count);

        if (allCubs[cubToKill].gameObject != null)
        {
            Instantiate(deadCubPrefab, allCubs[cubToKill].transform.position, Quaternion.identity);
            Destroy(allCubs[cubToKill]);
            runOnce = false;
        }
        else
        {
            //if something fucked up and it gave a null object, try again
            KillCub();
        }
    }
}

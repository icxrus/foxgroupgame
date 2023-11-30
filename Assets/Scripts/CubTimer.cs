using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubTimer : MonoBehaviour
{
    [SerializeField] private float timerDuration = 900.0f; // 15 mins in secs
    [SerializeField] private float currentTime;
    [SerializeField] private GameObject deadCubPrefab;
    private CubDataHolder cubData;
    private List<GameObject> allCubs = new List<GameObject>();
    private bool runOnce = false;

    void Start()
    {
        currentTime = timerDuration;
        cubData = GetComponent<CubDataHolder>();
        Invoke("GetCubs", 0.5f);
    }
    void GetCubs()
    {
        GameObject[] taggedCubs = GameObject.FindGameObjectsWithTag("Cub");

        foreach (GameObject cub in taggedCubs)
        {
            allCubs.Add(cub);
        }
    }

    void Update()
    {
        Debug.Log("allCubs is" + allCubs.Count);
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
    }
    void KillCub()
    {
        Debug.Log("KillCub ran");
        int cubToKill = Random.Range(0, allCubs.Count);

        if (allCubs[cubToKill].gameObject != null)
        {
            cubData.isCubDead[cubToKill] = true;
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

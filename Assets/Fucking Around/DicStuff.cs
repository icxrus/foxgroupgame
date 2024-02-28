using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DicStuff : MonoBehaviour
{
    public string which;
    Dictionary<string, Transform> cubDatas = new Dictionary<string, Transform>();

    private void Start()
    {
        cubDatas.Add("One", transform);
        cubDatas.Add("Two", transform.parent);
    }

    private void Update()
    {
        Testing(which);
    }
    void Testing(string num)
    {
        if (cubDatas.ContainsKey(num))
        {
            foreach (var data in cubDatas)
            {
                //that just shows the one data i loaded in
                Debug.Log($"the word {num} has this transform: {cubDatas[num]}");

                //that looks through all keys and their values
                Debug.Log($"Data is {data}");
            }
        }
    }
}

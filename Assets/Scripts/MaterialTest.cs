using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTest : MonoBehaviour
{
    public Material material;
    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.skybox = material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

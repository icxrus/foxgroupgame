using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubCollect : MonoBehaviour
{
    private CubFollow player;
    private float fogDensity;
    public delegate void CubCollected();
    public event CubCollected OnCubCollected;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<CubFollow>();
        OnCubCollected += player.AddCub;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CubTriggered();
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void AddCubFromScript()
    {
        CubTriggered();
    }

    void CubTriggered()
    {
        fogDensity = RenderSettings.fogDensity;

        float newFogDensity = fogDensity * 0.8f;
        RenderSettings.fogDensity = newFogDensity;

        Debug.Log("Fog Density: " + fogDensity);

        OnCubCollected?.Invoke();
    }

    /*public void RemoveCubFromScript()
    {
        fogDensity = RenderSettings.fogDensity;

        float newFogDensity = fogDensity * 1.8f;
        RenderSettings.fogDensity = newFogDensity;

        Debug.Log("Fog Density: " + fogDensity);
    }*/

}

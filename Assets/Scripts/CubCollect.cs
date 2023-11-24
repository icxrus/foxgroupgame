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
            fogDensity = RenderSettings.fogDensity;

            float newFogDensity = fogDensity * 0.8f;
            RenderSettings.fogDensity = newFogDensity;

            Debug.Log("Fog Density: " + fogDensity);

            OnCubCollected?.Invoke();
            Destroy(this.gameObject);
        }
    }

}
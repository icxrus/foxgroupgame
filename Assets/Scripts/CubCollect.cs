using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubCollect : MonoBehaviour
{
    private CubFollow player;
    private CubDataHolder cubData;
    private float fogDensity;
    public delegate void CubCollected();
    public event CubCollected OnCubCollected;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<CubFollow>();
        cubData = GameObject.FindWithTag("PuzzleManager").GetComponent<CubDataHolder>();
        OnCubCollected += player.AddCub;
        OnCubCollected += cubData.CubSaved;
    }
    private void Update()
    {
        //debug to spawn in a cub follower
        if (Input.GetKeyDown(KeyCode.P))
        {
            CubTriggered();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CubTriggered();
            //gameObject.GetComponent<BoxCollider>().enabled = false;
            Destroy(this.gameObject);
        }
    }
    public void CubTriggered()
    {
        fogDensity = RenderSettings.fogDensity;

        float newFogDensity = fogDensity * 0.8f;
        RenderSettings.fogDensity = newFogDensity;

        Debug.Log("Fog Density: " + fogDensity);

        OnCubCollected?.Invoke();
    }
}

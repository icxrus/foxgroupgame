using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubCollect : MonoBehaviour
{
    public delegate void CubCollection();
    public event CubCollection OnCubCollected;

    private CubFollow player;
    private CubDataHolder cubData;
    private float fogDensity;
    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<CubFollow>();
        cubData = GameObject.FindWithTag("PuzzleManager").GetComponent<CubDataHolder>();
        OnCubCollected += player.AddFollowerCub;
        OnCubCollected += cubData.IncreaseCubsSaved;
    }
    private void Update()
    {
        //Editor Specific Debugging
#if UNITY_EDITOR
        //debug to spawn in a cub follower
        if (Input.GetKeyDown(KeyCode.P))
        {
            CubCollected();
        }
#endif
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CubCollected();
            Destroy(this.gameObject);
        }
    }
    public void CubCollected()
    {
        fogDensity = RenderSettings.fogDensity;

        float newFogDensity = fogDensity * 0.8f;
        RenderSettings.fogDensity = newFogDensity;

        Debug.Log("Fog Density: " + fogDensity);

        OnCubCollected?.Invoke();
    }
}

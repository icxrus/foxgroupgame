using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubFollow : MonoBehaviour
{
    [SerializeField] private GameObject followerCubPrefab;
    [SerializeField] private List<Transform> cubsFollowing = new List<Transform>();
    [SerializeField] private Transform player;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float distanceBehind = 1.0f;
    [SerializeField] private float cubElevation = 0.25f;
    void Update()
    {
        if (cubsFollowing.Count == 1)
        {
            CubFollowPlayer();
        }
        else if (cubsFollowing.Count > 1)
        {
            CubFollowCub();
        }
    }
    public void AddFollowerCub()
    {
        Transform newCub = Instantiate(followerCubPrefab, player.position, Quaternion.identity).transform;
        cubsFollowing.Add(newCub);
    }

    void CubFollowPlayer()
    {
        //cub 0 follows player
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y + cubElevation, player.position.z) - player.forward * distanceBehind;
        Vector3 lookAtPosition = new Vector3(player.position.x, cubsFollowing[0].transform.position.y, player.position.z);

        cubsFollowing[0].transform.position = Vector3.Lerp(cubsFollowing[0].transform.position, targetPosition, followSpeed * Time.deltaTime);
        cubsFollowing[0].transform.LookAt(lookAtPosition);
    }
    void CubFollowCub()
    {
        //cub 1 to max follow cub before them
        for (int i = 0; i < cubsFollowing.Count - 1; i++)
        {
            Debug.Log("printing i = " + i) ;

            Transform cubToFollow = cubsFollowing[i];
            Transform cubFollowing = cubsFollowing[i + 1];

            Vector3 targetPosition = cubToFollow.position - cubToFollow.forward * distanceBehind;
            Vector3 lookAtPosition = new Vector3(cubToFollow.position.x, cubFollowing.transform.position.y, cubToFollow.position.z);

            cubFollowing.transform.position = Vector3.Lerp(cubFollowing.transform.position, targetPosition, followSpeed * Time.deltaTime);
            cubFollowing.transform.LookAt(lookAtPosition);
        }
    }
}

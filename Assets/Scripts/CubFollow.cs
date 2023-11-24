using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubFollow : MonoBehaviour
{
    [SerializeField] private GameObject cubPrefab;
    [SerializeField] private List<Transform> cubAmount = new List<Transform>();
    [SerializeField] private Transform player;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float distanceBehind = 1.0f;

    void Update()
    {
        if (cubAmount.Count > 0)
        {
            FollowPlayer();

            if (cubAmount.Count > 1)
            {
                FollowCub();
            }
        }
    }
    public void AddCub()
    {
        Transform newCub = Instantiate(cubPrefab, player.position, Quaternion.identity).transform;
        cubAmount.Add(newCub);
    }

    void FollowPlayer()
    {
        //cub 0 follows player
        Vector3 targetPosition = player.position - player.forward * distanceBehind;
        Vector3 lookAtPosition = new Vector3(player.position.x, cubAmount[0].transform.position.y, player.position.z);

        cubAmount[0].transform.position = Vector3.Lerp(cubAmount[0].transform.position, targetPosition, followSpeed * Time.deltaTime);
        cubAmount[0].transform.LookAt(lookAtPosition);
    }
    void FollowCub()
    {
        //cub 1 to max follow cub before them

        for (int i = 0; i < cubAmount.Count - 1; i++)
        {
            Debug.Log("printing i = " + i) ;

            Transform cubToFollow = cubAmount[i];

            Transform cubFollowing = cubAmount[i + 1];


            Vector3 targetPosition = cubToFollow.position - cubToFollow.forward * distanceBehind;
            Vector3 lookAtPosition = new Vector3(cubToFollow.position.x, cubFollowing.transform.position.y, cubToFollow.position.z);

            cubFollowing.transform.position = Vector3.Lerp(cubFollowing.transform.position, targetPosition, followSpeed * Time.deltaTime);
            cubFollowing.transform.LookAt(lookAtPosition);
        }
    }
}

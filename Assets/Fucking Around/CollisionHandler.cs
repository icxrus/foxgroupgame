using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollisionHandler
{
    public static bool CheckCollision(Collider other, string tagToCheck)
    {
        return other.gameObject.tag == tagToCheck;
    }
}

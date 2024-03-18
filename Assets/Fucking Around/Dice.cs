using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{

    public int RollDice(int gimmeNumber)
    {
        int roll = Random.Range(0, gimmeNumber);
        return roll;
    }
    
}

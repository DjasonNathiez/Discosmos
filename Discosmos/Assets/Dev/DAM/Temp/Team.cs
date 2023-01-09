using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField] private int teamID;

    public int TeamID
    {
        get { return teamID; }
        set { teamID = value; }
    }
}
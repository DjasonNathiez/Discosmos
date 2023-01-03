using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private GameObject blackHole;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Instantiate(blackHole, transform.position, transform.rotation);
        }
        
    }
}

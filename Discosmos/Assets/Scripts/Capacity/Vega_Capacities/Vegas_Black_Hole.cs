using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vegas_Black_Hole : MonoBehaviour
{
    
    private Vector3 forward;

    [SerializeField] private float forwardSpeed = 0;
    [SerializeField] private float duration = 0;
    [SerializeField] private GameObject blackHole;
    

    private void Start()
    {
        forward = transform.forward;
        blackHole.transform.position = transform.position;
    }
    
    private void Update()
    {
        transform.position += forward * (forwardSpeed * Time.deltaTime);
        if (duration <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            duration -= Time.deltaTime;
            
        }
    }
}

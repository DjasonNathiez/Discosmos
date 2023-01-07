using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vegas_Black_Hole : MonoBehaviour
{
    public PlayerManager sender;
    private Vector3 forward;
    public int damage;

    [SerializeField] private float forwardSpeed = 0;
    [SerializeField] private float duration = 0;
    [SerializeField] private GameObject blackHole;

    public void SetBlackHole(Vector3 direction, float duration, float speed, PlayerManager sender, int damage)
    {
        this.forward = direction;
        this.duration = duration;
        this.forwardSpeed = speed;
        this.damage = damage;
        
        this.sender = sender;
    }

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
            gameObject.SetActive(false);
        }
        else
        {
            duration -= Time.deltaTime;
            
        }
    }
}

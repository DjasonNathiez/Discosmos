using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Vegas_Black_Hole : MonoBehaviour
{
    public PlayerManager sender;
    private Vector3 forward;
    public int damage;

    [SerializeField] private float forwardSpeed = 0;
    [SerializeField] private float duration = 0;
    [SerializeField] private GameObject blackHole;

    private float durationTimer;
    private float serverTimeBackup;

    public void SetBlackHole(Vector3 direction, float duration, float speed, PlayerManager sender, int damage)
    {
        Debug.LogFormat("direction : {0} | speed : {1} | sender : {2}, damage : {3}", duration, speed, sender, damage);
        
        this.forward = direction;
        this.duration = duration;
        this.forwardSpeed = speed;
        this.damage = damage;
        
        this.sender = sender;
    }

    private void OnEnable()
    {
        GameAdministrator.OnServerUpdate += MoveBlackHole;
        serverTimeBackup = (float)PhotonNetwork.Time;
    }

    void MoveBlackHole()
    {
        if (durationTimer >= duration)
        {
            gameObject.SetActive(false);
            durationTimer = 0;
            GameAdministrator.OnServerUpdate -= MoveBlackHole;
        }
        else
        {
            transform.position += forward * (forwardSpeed * Time.deltaTime);
            durationTimer = (float)PhotonNetwork.Time - serverTimeBackup;
        }
    }
    
    private void Update()
    {
        transform.position += forward * (forwardSpeed * Time.deltaTime);
        
        if (durationTimer >= duration)
        {
            gameObject.SetActive(false);
            durationTimer = 0;

        }
        else
        {
            durationTimer += Time.deltaTime;
        }
    }
}

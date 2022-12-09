using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private GameObject tower1;
    [SerializeField] private GameObject tower2;
    
    private int numberOfTowersAlive;

    [SerializeField] private int team;
    
    [SerializeField] private float forceBoost;
    private float currentForce;
    [SerializeField] private float cooldown;
    private float cooldownTimer;
    
    private PlayerController playerController;
    private Team teamScript;
    
    
    private void Start()
    {
        if (tower1.activeSelf && tower2.activeSelf)
        {
            numberOfTowersAlive = 2;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        playerController = other.GetComponentInChildren<PlayerController>();
        // teamScript = other.GetComponent<Team>();
        
        if (playerController != null /*&& teamScript != null*/)
        {
            Debug.Log("Player is not null");
            /*if ( team )
            {*/
                if (numberOfTowersAlive == 2)
                {
                    Debug.Log("Both towers are alive");
                    //if the cooldown is over, boost the player
                    if (cooldownTimer <= 0)
                    {
                        Debug.Log("Boosting player");
                        ForceBoost();
                    }
                }
            //}
            else
            {
                //block the player from going through the wall
                playerController.BlockPlayer();
            }
        }
    }

    private void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private void ForceBoost()
    {
        currentForce = playerController.GetForce();
        playerController.SetForce(Mathf.Lerp(currentForce, currentForce + forceBoost, 1));
        cooldownTimer = cooldown;
    }
}

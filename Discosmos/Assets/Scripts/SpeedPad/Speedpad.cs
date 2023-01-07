using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedpad : MonoBehaviour
{
    //make it so when an object enters the trigger, it checks if it has the PlayerController script and put speed to 10 if the player goes in the same direction the speedpad is facing
    [SerializeField] private PlayerController playerController;
    private void OnTriggerEnter(Collider other)
    {
        //draw the transform.forward of the speedpad
        if (other.gameObject.GetComponent<PlayerController>())
        {
            Debug.Log("Enter");
            playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.SpeedPadTrigger(true,transform);
            
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            Debug.Log("Exit");
            playerController.SpeedPadTrigger(false,transform);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedpad : MonoBehaviour
{
    //make it so when an object enters the trigger, it checks if it has the PlayerController script and put speed to 10 if the player goes in the same direction the speedpad is facing
    
    [SerializeField] private float speed = 10;
    [SerializeField] private float slow = 2;
    
    [SerializeField] private float precision;
    
    private float playerspeed;
    private void OnTriggerEnter(Collider other)
    {
        //draw the transform.forward of the speedpad
        Debug.DrawRay(transform.position, transform.forward, Color.blue, 50);
        Debug.Log("Enter");
        if (other.gameObject.GetComponent<PlayerController>())
        {
            playerspeed = other.gameObject.GetComponent<PlayerController>().baseSpeed;
            //if the player is approaching the speedpad from the same direction as the speedpad is facing, increase the speed
            if (Vector3.Dot(other.gameObject.transform.forward, transform.forward) > precision)
            {
                other.gameObject.GetComponent<PlayerController>().baseSpeed *= speed;
            }
            else
            {
                other.gameObject.GetComponent<PlayerController>().baseSpeed *= 1/slow;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
        if (other.gameObject.GetComponent<PlayerController>())
        {
            other.gameObject.GetComponent<PlayerController>().baseSpeed = playerspeed;
        }
    }
}

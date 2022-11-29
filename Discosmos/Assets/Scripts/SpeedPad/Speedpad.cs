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
        if (other.gameObject.GetComponent<PlayerController>())
        {
            //if the player is going in the same direction as the speedpad, set the speed to 10
            if (Vector3.Dot(other.transform.forward, transform.forward) > precision)
            {
                playerspeed = speed;
            }
            //if the player is going in the opposite direction as the speedpad, set the speed to 2
            else
            {
                playerspeed = slow;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            other.gameObject.GetComponent<PlayerController>().speed = playerspeed;
        }
    }
}

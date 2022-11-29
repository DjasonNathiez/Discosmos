using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invis : MonoBehaviour
{
    //when a gameObject with this script PlayerController enters the trigger, set the alpha of the sprite to 0.5
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            Debug.Log("Player entered the trigger");
            //disable te mesh renderer of the parent object
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    
    //when a gameObject with this script PlayerController exits the trigger, set the alpha of the sprite to 1
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            //enable the mesh renderer of the parent object
            other.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}

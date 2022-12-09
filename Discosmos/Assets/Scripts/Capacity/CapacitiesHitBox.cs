using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CapacitiesHitBox : MonoBehaviour
{
    [SerializeField] private PlayerManager owner;
    public List<int> idOnIt;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PhotonView>())
        {
            idOnIt.Add(PhotonView.Get(other.gameObject).ViewID);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PhotonView>())
        {
            idOnIt.Remove(PhotonView.Get(other.gameObject).ViewID);
        }
    }
}

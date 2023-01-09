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
        Targetable targetable = other.GetComponent<Targetable>();

        if (targetable && !idOnIt.Contains(targetable.photonID))
        {
            idOnIt.Add(targetable.photonID);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        Targetable targetable = other.GetComponent<Targetable>();

        if (targetable && idOnIt.Contains(targetable.photonID))
        {
            idOnIt.Remove(targetable.photonID);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;

public class CenterEffects : MonoBehaviour
{
    [SerializeField] private float succForce;
    [SerializeField] private float damages;
    [SerializeField] private float tickDamage;
    [SerializeField] private float timer;
    [SerializeField] private float radius;
    
    private SphereCollider _collider;
    [SerializeField] private List<Rigidbody> _rigidbodies;
    private Rigidbody _rigidbody;
    private List<int> hitID = new List<int>();
    private PlayerManager playerManager;
    private MinionsController minionsController;
    private void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
        _rigidbodies = new List<Rigidbody>();
    }
    
    private void Update()
    {
        _collider.radius = radius;
        
        if (_rigidbodies.Count > 0)
        {
            foreach (var rb in _rigidbodies)
            {
                rb.AddForce((transform.position - rb.transform.position).normalized * (succForce * Time.deltaTime));
            }
            if (timer > 0)
            {
                timer -= Time.deltaTime;

            }
            else
            {
                foreach (var rb in _rigidbodies)
                {
                    rb.AddForce((transform.position - rb.transform.position).normalized * (succForce * Time.deltaTime));
                    if (rb.transform.parent.GetComponent<PlayerManager>())
                    {
                        playerManager = rb.transform.parent.GetComponent<PlayerManager>();
                        minionsController = null;
                    }
                    else if (rb.transform.GetComponent<MinionsController>())
                    {
                        playerManager = null;
                        minionsController = rb.transform.GetComponent<MinionsController>();
                    }
                    
                    if (playerManager != null)
                    {
                        
                        playerManager.DealDamage(new int[]{playerManager.photonView.ViewID},(int)damages);

                    }
                    else if (minionsController != null)
                    {
                        
                        minionsController.DealDamage(new int[]{minionsController.photonView.ViewID},(int)damages);
                        Debug.Log("Minion");
                        
                    }
                    Debug.DrawLine(transform.position, rb.transform.position, Color.blue);
                }
                timer = tickDamage;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Targetable>())
        {
            _rigidbody = other.GetComponent<Targetable>().GetComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                _rigidbodies.Add(_rigidbody);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Targetable>())
        {
            _rigidbody = other.GetComponent<Targetable>().GetComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                _rigidbodies.Remove(_rigidbody);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

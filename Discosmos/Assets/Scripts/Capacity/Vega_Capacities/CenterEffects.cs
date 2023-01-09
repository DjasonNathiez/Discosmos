using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;

public class CenterEffects : MonoBehaviour
{
    public Vegas_Black_Hole head;

    [SerializeField] private float succForce;
    [SerializeField] private float damages;
    [SerializeField] private float tickDamage;
    [SerializeField] private float timer;
    [SerializeField] private float radius;
    
    private SphereCollider _collider;
    [SerializeField] private List<Rigidbody> _rigidbodies;
    private Rigidbody _rigidbody;
    private List<int> hitID;
    
    private void Start()
    {
        hitID = new List<int>();
        
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
                
                if(hitID.Count != 0) head.sender.DealDamage(hitID.ToArray(),head.damage);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        
        if(controller  && controller.manager != head.sender)
        {
            _rigidbody = controller.GetComponent<Rigidbody>();
            
            if (_rigidbody != null)
            {
                _rigidbodies.Add(_rigidbody);
            }
            
            hitID.Add(controller.manager.photonView.ViewID);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        
        PlayerController controller = other.GetComponent<PlayerController>();

        if(controller && controller.manager != head.sender)
        {
            _rigidbody = controller.GetComponent<Rigidbody>();
            
            if (_rigidbody != null)
            {
                _rigidbodies.Remove(_rigidbody);
            }
            
            hitID.Remove(controller.manager.photonView.ViewID);
        }
    }
}

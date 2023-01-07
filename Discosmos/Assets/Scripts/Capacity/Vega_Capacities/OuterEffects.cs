using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterEffects : MonoBehaviour
{
    public Vegas_Black_Hole head;
    
    [SerializeField] private float succForce;
    [SerializeField] private float slowForce;
    [SerializeField] private float radius;
    
    private SphereCollider _collider;
    [SerializeField] private List<Rigidbody> _rigidbodies;
    private Rigidbody _rigidbody;

    
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
                //set the velocity of the rigidbody to the direction of the center of the sphere
                rb.velocity = (transform.position - rb.transform.position).normalized * succForce;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerManager manager = other.GetComponent<PlayerManager>();
        
        if(manager && manager != head.sender)
        {
            _rigidbody = manager.PlayerController.GetComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                _rigidbodies.Add(_rigidbody);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        
        PlayerManager manager = other.GetComponent<PlayerManager>();

        if(manager && manager != head.sender)
        {
            _rigidbody = manager.PlayerController.GetComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                _rigidbodies.Remove(_rigidbody);
            }
        }
    }
}

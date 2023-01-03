using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEffects : MonoBehaviour
{
    [SerializeField] private float speedBoost;
    [SerializeField] private float length;
    [SerializeField] private float width;
    private Vector3 backwards;
    private BoxCollider boxCollider;
    [SerializeField] private List<Rigidbody> _rigidbodies;
    private Rigidbody _rigidbody;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(width, width, length);
        boxCollider.center = new Vector3(0, 0, length / 2);
        boxCollider.isTrigger = true;
        _rigidbodies = new List<Rigidbody>();
        boxCollider.transform.localPosition = new Vector3(0, 0, -length / 2);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            _rigidbody = other.GetComponent<PlayerController>().GetComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                _rigidbodies.Add(_rigidbody);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            _rigidbody = other.GetComponent<PlayerController>().GetComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                _rigidbodies.Remove(_rigidbody);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, 0, length / 2), new Vector3(width, width, length));
    }
}

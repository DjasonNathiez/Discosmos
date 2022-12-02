using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerModule : MonoBehaviour
{
    public bool _isInBrush;
    float shaderTransitionValue = 1;
    public float timeInBrume;
    [SerializeField] private float detectionRange;
    

    [Header("Collider")] 
    private SphereCollider colliderPlayer;

    private void Start()
    {
        colliderPlayer = GetComponent<SphereCollider>();
        colliderPlayer.radius = detectionRange;
    }

    public bool isInBrush
    {
        get => _isInBrush; set
        {
            _isInBrush = value;
        }
    }
}

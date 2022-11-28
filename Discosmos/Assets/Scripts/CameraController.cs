using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector2 rotationOffset;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float cameraZoom;

    [SerializeField] private bool cameraLock = true;
    
    Vector3 nextPos;
    
    private void Start()
    {
        transform.rotation = Quaternion.Euler(rotationOffset);
        transform.position += offset;
    }
    
    
    private void OnToggleCameraLock(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        cameraLock = !cameraLock;
        Debug.Log("Camera Lock Toggled");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            cameraLock = !cameraLock;
        }
        
    }

    private void LateUpdate()
    {
        UpdateCamera();
    }


    private void UpdateCamera()
    {

        if (cameraLock)
        {
            nextPos = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, nextPos, /*tick * */ smoothSpeed);
        }
        else
        {
            nextPos = transform.position;

            if (Input.mousePosition.x >= Screen.width - 1)
            {
                nextPos += transform.right * cameraSpeed;
            }

            if (Input.mousePosition.x <= 0)
            {
                nextPos -= transform.right * cameraSpeed;
            }

            if (Input.mousePosition.y >= Screen.height - 1)
            {
                nextPos += transform.up * cameraSpeed;
            }

            if (Input.mousePosition.y <= 0)
            {
                nextPos -= transform.up * cameraSpeed;
            }

            transform.position = Vector3.Lerp(transform.position, nextPos, /*tick * */ smoothSpeed);
        }
        transform.rotation = Quaternion.Euler(rotationOffset);
        transform.position += transform.forward * cameraZoom;

        
    }
}

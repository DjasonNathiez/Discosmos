using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CameraController : MonoBehaviour
{
    
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector2 rotationOffset;
    [SerializeField] private float cameraSpeed;
    
    [Range(0,4)] [SerializeField] private float cameraZoom;

    private bool cameraLock = true;
    private Vector3 nextPos;
    private Vector3 forward;
    private Vector3 right;
    
    private void Start()
    {
        //transform rotation but just the y
        transform.rotation = Quaternion.Euler(0, rotationOffset.y, 0);
        forward = transform.forward;
        right = transform.right;
        transform.rotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, 0);
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
            transform.position = Vector3.Lerp(transform.position, nextPos, smoothSpeed);
        }
        else
        {
            nextPos = transform.position;

            if (Input.mousePosition.x >= Screen.width - 1)
            {
                nextPos += right * cameraSpeed;
            }

            if (Input.mousePosition.x <= 0)
            {
                nextPos -= right * cameraSpeed;
            }

            if (Input.mousePosition.y >= Screen.height - 1)
            {
                //while ignoring rotation on the x axis move the camera forward
                nextPos += forward * cameraSpeed;
            }

            if (Input.mousePosition.y <= 0)
            {
                nextPos -= forward * cameraSpeed;
            }
        }
        //if scroll wheel is used zoom in or out
        if (Input.mouseScrollDelta.y != 0)
        {
            cameraZoom -= Input.mouseScrollDelta.y * 0.1f;
        }
        //lerp Vector3.one * Mathf.Clamp(cameraZoom,0,4)
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * Mathf.Clamp(cameraZoom, 0, 4), 0.125f);
        transform.position = Vector3.Lerp(transform.position, nextPos, smoothSpeed);

        
    }
}

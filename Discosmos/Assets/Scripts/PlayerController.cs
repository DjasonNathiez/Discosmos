using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    
    private NavMeshAgent agent;
    [SerializeField] private float speed;

    private Ray ray;
    private RaycastHit hit;
    
    //if one of these is true, the other is false
    [SerializeField] private bool moveToClickWithNavMesh, followMouseWithNavMesh, followMouseClickWithNavMesh, followMouseClickWithoutNavMesh, followMouseWithoutNavMesh, moveToClickWithoutNavMesh;

    private Vector3 goal;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }
    
    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (moveToClickWithNavMesh)
        {
            agent.enabled = true;
            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    agent.SetDestination(hit.point);
                }
            }
            else
            {
                agent.SetDestination(transform.position);
            }
        }
        else if (followMouseWithNavMesh)
        {
            agent.enabled = true;
            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
        else if (followMouseClickWithNavMesh)
        {
            agent.enabled = true;
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    agent.SetDestination(hit.point);
                }
            }
        }
        else if (followMouseClickWithoutNavMesh)
        {
            agent.enabled = false;
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    goal = hit.point;
                }
            }
            else
            {
                //stop moving and stop the lerp
                transform.position = transform.position;
                hit.point = transform.position;
            }
            transform.position = Vector3.Lerp(transform.position, goal, speed * Time.deltaTime);
        }
        else if (followMouseWithoutNavMesh)
        {
            agent.enabled = false;
            if (Physics.Raycast(ray, out hit))
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, speed * Time.deltaTime);
            }
            else
            {
                //stop moving and stop the lerp
                transform.position = transform.position;
                hit.point = transform.position;
            }
        }
        else if (moveToClickWithoutNavMesh)
        {
            agent.enabled = false;
            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    transform.position = Vector3.Lerp(transform.position, hit.point, speed * Time.deltaTime);
                }
                else
                {
                    //stop moving and stop the lerp
                    transform.position = transform.position;
                    hit.point = transform.position;
                }
            }
        }
        if (moveToClickWithNavMesh)
        {
            followMouseWithNavMesh = false;
            followMouseClickWithNavMesh = false;
            followMouseClickWithoutNavMesh = false;
            followMouseWithoutNavMesh = false;
            moveToClickWithoutNavMesh = false;
        }
        else if (followMouseWithNavMesh)
        {
            followMouseClickWithNavMesh = false;
            followMouseClickWithoutNavMesh = false;
            followMouseWithoutNavMesh = false;
            moveToClickWithoutNavMesh = false;
            moveToClickWithNavMesh = false;
        }
        else if (followMouseClickWithNavMesh)
        {
            followMouseClickWithoutNavMesh = false;
            followMouseWithoutNavMesh = false;
            moveToClickWithoutNavMesh = false;
            moveToClickWithNavMesh = false;
            followMouseWithNavMesh = false;
        }
        else if (followMouseClickWithoutNavMesh)
        {
            followMouseWithoutNavMesh = false;
            moveToClickWithoutNavMesh = false;
            moveToClickWithNavMesh = false;
            followMouseWithNavMesh = false;
            followMouseClickWithNavMesh = false;
        }
        else if (followMouseWithoutNavMesh)
        {
            moveToClickWithoutNavMesh = false;
            moveToClickWithNavMesh = false;
            followMouseWithNavMesh = false;
            followMouseClickWithNavMesh = false;
            followMouseClickWithoutNavMesh = false;
        }
        else if (moveToClickWithoutNavMesh)
        {
            moveToClickWithNavMesh = false;
            followMouseWithNavMesh = false;
            followMouseClickWithNavMesh = false;
            followMouseClickWithoutNavMesh = false;
            followMouseWithoutNavMesh = false;
        }
        
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hit.point, 0.1f);
    }
    
    
    
}

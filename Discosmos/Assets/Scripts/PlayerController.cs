using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    
    private NavMeshAgent agent;
    [SerializeField] public float speed;

    private Ray ray;
    private Ray rayCam;
    private RaycastHit hit;
    private Vector3 goal;
    
    private bool attacking =false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }
    
    private void Update()
    {
        MovementTypeSwitch();
        agent.speed = speed;
        Debug.DrawLine(transform.position, agent.destination, Color.yellow);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hit.point, 0.1f);
    }
    
    
    private enum MovementType
    {
        moveToClickWithNavMesh,
        followMouseWithNavMesh,
        followMouseClickWithNavMesh,
        moveToClickbutKeepDirectionWithNavMesh,
        followMouseClickWithoutNavMesh,
        followMouseWithoutNavMesh,
        moveToClickWithoutNavMesh,
        moveToClickbutKeepDirectionWithoutNavMesh
    }
    
    [SerializeField] private MovementType movementType;
    
    private void MovementTypeSwitch()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        switch (movementType)
        {
            case MovementType.moveToClickWithNavMesh:
                MoveToClickUsingTheNavMesh();
                break;
            case MovementType.followMouseWithNavMesh:
                FollowTheMouseUsingNavMesh();
                break;
            case MovementType.followMouseClickWithNavMesh:
                FollowTheMouseWhenPressedUsingNavMesh();
                break;
            case MovementType.moveToClickbutKeepDirectionWithNavMesh:
                MoveToClickButKeepDirectionUsingNavMesh();
                break;
            case MovementType.followMouseClickWithoutNavMesh:
                FollowMousePosWhenPressedNoNavMesh();
                break;
            case MovementType.followMouseWithoutNavMesh:
                FollowMousePosNoNavMesh();
                break;
            case MovementType.moveToClickWithoutNavMesh:
                MoveToClickNoNavMesh();
                break;
            case MovementType.moveToClickbutKeepDirectionWithoutNavMesh:
                MoveToClickButKeepDirectionNoNavMesh();
                break;
        }
    }

    private void MoveToClickButKeepDirectionNoNavMesh()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                agent.enabled = false;
                goal = hit.point;
                goal.y = transform.position.y;
            }
        }
        if (Vector3.Distance(transform.position, goal) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, goal, speed * Time.deltaTime);
            transform.LookAt(goal);
        }
        else
        {
            goal = transform.position + transform.forward * 10;
        }
    }

    


    private void MoveToClickNoNavMesh()
    {
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
        {
            agent.enabled = false;
            goal = hit.point;
            goal.y = transform.position.y;
        }
        GoToNoNavMesh();
    }

    

    private void FollowMousePosNoNavMesh()
    {
        if (Physics.Raycast(ray, out hit)) 
        {
            agent.enabled = false;
            goal = hit.point;
            goal.y = transform.position.y;
        }
        else
        {
            transform.position = transform.position;
            goal = transform.position;
        }
        GoToNoNavMesh();
    }

    private void FollowMousePosWhenPressedNoNavMesh()
    {
        if (Input.GetMouseButton(0) && Physics.Raycast(ray, out hit))
        {
            agent.enabled = false;
            goal = hit.point;
            goal.y = transform.position.y;
        }
        else
        {
            transform.position = transform.position;
            goal = transform.position;
        }
        GoToNoNavMesh();
    }
    
    private void MoveToClickButKeepDirectionUsingNavMesh()
    {
        agent.enabled = true;
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                goal = hit.point;
                agent.SetDestination(goal);
            }
        }
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.ResetPath();
            goal = transform.position + transform.forward * 10;
            agent.SetDestination(goal);

        }
        
        for (int i = 0; i < agent.path.corners.Length - 1; i++)
        {
            Debug.DrawLine(agent.path.corners[i], agent.path.corners[i + 1], Color.red);
        }
        
    }

    private void FollowTheMouseWhenPressedUsingNavMesh()
    {
        agent.enabled = true;
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit)) agent.SetDestination(hit.point);
    }

    private void FollowTheMouseUsingNavMesh()
    {
        agent.enabled = true;
        if (Physics.Raycast(ray, out hit)) agent.SetDestination(hit.point);
    }

    private void MoveToClickUsingTheNavMesh()
    {
        agent.enabled = true;
        if (Input.GetMouseButton(0) && Physics.Raycast(ray, out hit)) agent.SetDestination(hit.point);
        else agent.SetDestination(transform.position);
    }
    
    
    private void GoToNoNavMesh()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = Vector3.MoveTowards(transform.position, goal, speed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.LookAt(goal);
    }
}

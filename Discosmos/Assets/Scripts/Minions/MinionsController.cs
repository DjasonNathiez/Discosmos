using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionsController : MonoBehaviour
{
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private float range;
    [SerializeField] private float speed;

    private NavMeshAgent agent;
    private int currentWaypoint = 0;
    private bool isMoving = false;
    private bool isAttacking = false;

    private Team team;

    private List<GameObject> entitiesInRange = new List<GameObject>();
    public GameObject target;
    private Vector3 targetPosition;
    
    private Collider[] colliders;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        team = GetComponent<Team>();
        //Find the gameObject named "WaypointsTeam" + team.teamID and add the transforms of its children to the waypoints array
        GameObject waypoints = GameObject.Find("WaypointsTeam" + team.TeamID);
        _waypoints = waypoints.GetComponentsInChildren<Transform>(); 
        _waypoints = _waypoints[1..];
        System.Array.Sort(_waypoints, (x, y) => Vector3.Distance(transform.position, x.position).CompareTo(Vector3.Distance(transform.position, y.position)));
    }

    private void Update()
    {
        //while there are no GameObjects with the team != this.team in the entitiesInRange list and the currentWaypoint is not the last waypoint move to the next waypoint else move In Range and attack
        if (entitiesInRange.Count == 0 && currentWaypoint < _waypoints.Length)
        {
            MoveToWaypoint();
        }
        else
        {
            MoveInRange();
        }
    }

    private void MoveToWaypoint()
    {
        agent.SetDestination(_waypoints[currentWaypoint].position);
        if (Vector3.Distance(transform.position, _waypoints[currentWaypoint].position) < 1f)
        {
            currentWaypoint++;
        }

    }
    
    private void MoveInRange()
    {
        colliders = Physics.OverlapSphere(transform.position, range);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.GetComponent<Team>() != null)
            {
                if (collider.gameObject.GetComponent<Team>().TeamID != team.TeamID)
                {
                    target = collider.gameObject;
                    targetPosition = target.transform.position;
                    agent.SetDestination(targetPosition);
                    if (Vector3.Distance(transform.position, targetPosition) < 1f)
                    {
                        Attack();
                    }
                }
            }
        }
    }

    private void Attack()
    {
        Debug.Log("Attack");
    }
}
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsInRange = new List<GameObject>();
    [SerializeField] private float laviolence = 10f;
    
    [SerializeField] private float grabRadius = 1f; 
    [SerializeField] private float grabAngle = 30f; 
    [SerializeField] private float grabRotation = 0f;
    [SerializeField] private Vector3 grabOffset = Vector3.zero;
    [SerializeField] private float duration = 0.5f; 
    Vector3 point1;
    Vector3 point2;

    private void Start()
    {
        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = grabRadius;
        //add the offset to the collider
        sphereCollider.center = grabOffset;
        //set 2 points using the grabAngle on the x and z axis of the sphere collider at the edge of the radius
        point1 = new Vector3(Mathf.Cos(grabAngle * Mathf.Deg2Rad) * grabRadius, 0, Mathf.Sin(grabAngle * Mathf.Deg2Rad) * grabRadius);
        point2 = new Vector3(Mathf.Cos(grabAngle * Mathf.Deg2Rad) * grabRadius, 0, -Mathf.Sin(grabAngle * Mathf.Deg2Rad) * grabRadius);
        //rotate the points by the grabRotation
        point1 = Quaternion.Euler(0, grabRotation+180, 0) * point1;
        point2 = Quaternion.Euler(0, grabRotation+180, 0) * point2;
    }

    private void Update()
    {
        foreach (GameObject obj in objectsInRange)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce((transform.position - obj.transform.position) * laviolence, ForceMode.Impulse);
                if (Vector3.Distance(transform.position, obj.transform.position) < 0.1f)
                {
                    rb.velocity = Vector3.zero;
                }
            }
        }
        
        if (duration <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            duration -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() == null) return;
        if (objectsInRange.Contains(other.gameObject)) return;
        if (IsPointInTriangle(transform.position, transform.position + point1, transform.position + point2, other.transform.position))
        {
            objectsInRange.Add(other.gameObject);
        }
    }

    private static bool IsPointInTriangle(Vector3 transformPosition, Vector3 position, Vector3 vector3, Vector3 transformPosition1)
    {
        if (Vector3.Cross(position - transformPosition, transformPosition1 - transformPosition).y > 0)
        {
            return false;
        }
        if (Vector3.Cross(vector3 - position, transformPosition1 - position).y > 0)
        {
            return false;
        }
        return !(Vector3.Cross(transformPosition - vector3, transformPosition1 - vector3).y > 0);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() != null)
        {
            objectsInRange.Remove(other.gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var position = transform.position;
        Gizmos.DrawWireSphere(position + grabOffset, grabRadius);
        Gizmos.color = Color.green;
        point1 = new Vector3(Mathf.Cos(grabAngle * Mathf.Deg2Rad) * grabRadius, 0, Mathf.Sin(grabAngle * Mathf.Deg2Rad) * grabRadius);
        point2 = new Vector3(Mathf.Cos(grabAngle * Mathf.Deg2Rad) * grabRadius, 0, -Mathf.Sin(grabAngle * Mathf.Deg2Rad) * grabRadius);
        point1 = Quaternion.Euler(0, grabRotation+180, 0) * point1;
        point2 = Quaternion.Euler(0, grabRotation+180, 0) * point2;
        Gizmos.DrawWireSphere(position + grabOffset + point1, 0.1f);
        Gizmos.DrawWireSphere(position + grabOffset + point2, 0.1f);
        Gizmos.DrawLine(position + grabOffset + point1, position + grabOffset + point2);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(position + grabOffset, position + grabOffset + point1);
        Gizmos.DrawLine(position + grabOffset, position + grabOffset + point2);
    }
}

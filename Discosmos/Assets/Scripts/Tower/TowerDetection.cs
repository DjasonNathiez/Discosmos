using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDetection : MonoBehaviour
{
    //this script is attached to the tower detection object which is a child of the tower object and is used to detect enemies in range of the tower 
    //every game objects that are named "*Player*" are considered as enemies

    //set the range of the tower
    public float detectionRadius = 5f;

    //create a list of enemies in range
    public List<GameObject> enemiesInRange = new List<GameObject>();
    public GameObject target;
    private Vector3 topOfTower;

    //debug values
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private bool hasEnnemiesInRange;
    [SerializeField] private bool rayCastHit;

    [SerializeField] private bool rotationLocked = true;
    /*il faudra créer une variable pour savoir la team ici*/

    private void Start()
    {
        topOfTower = transform.parent.position;
    }

    void Update()
    {
        enemiesInRange.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.name.Contains("Player") /* && si la team du gameObject = variable team*/)
            {
                enemiesInRange.Add(collider.gameObject);
            }
        }

        if (enemiesInRange.Count > 0)
        {
            hasEnnemiesInRange = true;
            if (target == null || !enemiesInRange.Contains(target))
            {
                target = enemiesInRange[0];
            }

            RaycastHit hit;
            if (Physics.Raycast(topOfTower, target.transform.position - topOfTower, out hit))
            {
                if (!rotationLocked)
                {
                    targetPosition = new Vector3(target.transform.position.x, transform.position.y,
                        target.transform.position.z);
                    transform.LookAt(new Vector3(target.transform.position.x, transform.position.y,
                        target.transform.position.z));
                }

                Debug.DrawLine(this.transform.position, target.transform.position, Color.yellow);
            }
        }
        else
        {
            target = null;
            hasEnnemiesInRange = false;
        }
    }

}
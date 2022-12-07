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
        //set the top of the tower to the position of the child of the tower detection object named "TowerShoot"
        topOfTower = transform.GetChild(0).position;
    }

    void Update()
    {
        //clear the list of enemies in range
        enemiesInRange.Clear();
        //get all the colliders in the detection radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        //loop through all the colliders
        foreach (Collider collider in colliders)
        {
            //if the collider has "Player" in its name
            if (collider.name.Contains("Player") /* && si la team du gameObject = variable team*/)
            {
                //add the collider to the list of enemies in range
                enemiesInRange.Add(collider.gameObject);
            }
        }

        if (enemiesInRange.Count > 0)
        {
            hasEnnemiesInRange = true;
            //if there isn't a target yet set the first enemy in the list as the target or if the target is not in the list of enemies in range set the first enemy in the list as the target
            if (target == null || !enemiesInRange.Contains(target))
            {
                target = enemiesInRange[0];
            }

            //launch a raycast from the top of the tower to the enemy
            RaycastHit hit;
            if (Physics.Raycast(topOfTower, target.transform.position - topOfTower, out hit))
            {
                //rotate the tower to face the enemy but to not change the y rotation
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

    //draw the detection radius in the scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
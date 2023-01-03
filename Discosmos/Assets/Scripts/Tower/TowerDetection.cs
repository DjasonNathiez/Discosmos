using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDetection : MonoBehaviour
{
    public float detectionRadius = 5f;
    private Collider[] collidersDetection;
    public List<GameObject> enemiesInRange = new List<GameObject>();
    public GameObject target;
    private Vector3 topOfTower;

    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private bool hasEnnemiesInRange;
    [SerializeField] private bool rayCastHit;

    [SerializeField] private bool rotationLocked = true;
    /*il faudra créer une variable pour savoir la team ici*/
    
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private float timer = 0f;

    private void Start()
    {
        topOfTower = transform.parent.position;
    }

    void Update()
    {
        enemiesInRange.Clear();
        collidersDetection = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider collider in collidersDetection)
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
        
        if (hasEnnemiesInRange)
        {
            timer += Time.deltaTime;
            if (timer >= cooldown)
            {
                timer = 0;
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        Debug.Log("Tower shoot");
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
    }
}
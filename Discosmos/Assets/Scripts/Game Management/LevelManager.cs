using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform spawnPoint;
    
    void Start()
    {
        FindObjectOfType<PlayerManager>().transform.position = spawnPoint.position;
        FindObjectOfType<PlayerController>()._camera = Camera.main;
    }
}

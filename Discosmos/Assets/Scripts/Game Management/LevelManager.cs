using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform spawnPoint;
    
    void Start()
    {
        GameAdministrator.instance.localPlayer.PlayerController.transform.position = spawnPoint.position;
    }
}

using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public void CallAttack()
    {
        playerController.OnAttack();
    }

    public void CallMimiLaser()
    {
        //playerController.OnCapacityPerfom(MimiLaser);
    }
    
    public void CallMimiAttackFX()
    {
        playerController.playerManager.CallFX(VisualEffects.MimiAutoAttack);
    }

    public void CallMimiLaserVFX()
    {
        playerController.playerManager.CallFX(VisualEffects.MimiLaser);
    }
}

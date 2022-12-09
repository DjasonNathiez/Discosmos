using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [Header("Mimi Hit Box")]
    public CapacitiesHitBox laserHitBox;

    public void CallAttack()
    {
        playerController.OnAttack();
    }

    public void CallMimiLaser()
    {
        playerController.OnCapacityPerformed(Capacities.MIMI_Laser, laserHitBox.idOnIt.ToArray());
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

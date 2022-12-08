using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public void CallAttack()
    {
        playerController.OnAttack();
    }
    
    public void CallAttackFX()
    {
        playerController.playerManager.CallFX(VisualEffects.MimiAutoAttack);
    }
}

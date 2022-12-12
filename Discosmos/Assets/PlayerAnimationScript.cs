using System;
using Toolbox.Variable;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAnimationScript : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [Header("Mimi Hit Box")]
    public CapacitiesHitBox laserHitBox;
    
    [Header("Shaking")]
    public bool shaking;
    public float shakingForce;
    public float shakingTime;
    public float shakingDuration;
    public Vector3 truePos;
    public float previousShake = 1;
    public float nextShakeTimer = 0.02f;
    public float shakeFrequency = 0.02f;

    [Header("Teams Skins")] 
    [SerializeField] private SkinnedMeshRenderer[] meshRenderers;
    [SerializeField] private Material mimiPurple;
    [SerializeField] private Material mimiGreen;
    

    public void Awake()
    {
        truePos = transform.localPosition;
    }

    public void CallAttack()
    {
        playerController.OnAttack();
    }

    public void CallMimiLaser()
    {
        playerController.OnCapacityPerformed(Capacities.MIMI_Laser, laserHitBox.idOnIt);
    }
    
    public void CallMimiAttackFX()
    {
        playerController.manager.CallFX(VisualEffects.MimiAutoAttack);
    }

    public void CallMimiLaserVFX()
    {
        playerController.manager.CallFX(VisualEffects.MimiLaser);
    }

    public void Shake(float force, float time)
    {
        shakingForce = force;
        shakingTime = time;
        shakingDuration = time;
        shaking = true;
    }

    public void SetTeamModel(Enums.Teams teams)
    {
        switch (teams)
        {
            case Enums.Teams.Green:
                foreach (var meshRenderer in meshRenderers)
                {
                    meshRenderer.material = mimiGreen;
                }
                break;
            
            case Enums.Teams.Pink:
                foreach (var meshRenderer in meshRenderers)
                {
                    meshRenderer.material = mimiPurple;
                }
                break;
        }
    }
    private void Update()
    {
        if (shaking)
        {
            if (shakingTime > 0)
            {
                shakingTime -= Time.deltaTime;
                if (nextShakeTimer > 0)
                {
                    nextShakeTimer -= Time.deltaTime;
                }
                else
                {
                    nextShakeTimer = shakeFrequency;
                    Vector2 shake = new Vector2(Random.Range(0.2f, 1f) * shakingForce * (shakingTime /(shakingDuration / 0.7f) + 0.3f)* previousShake, Random.Range(0.2f, 1f) * shakingForce * (shakingTime /(shakingDuration / 0.7f) + 0.3f) * previousShake);
                    transform.localPosition = new Vector3(truePos.x + shake.x, truePos.y, truePos.z + shake.y);
                    previousShake = -previousShake;
                }
            }
            else
            {
                transform.localPosition = truePos;
                shakingTime = 0;
                shaking = false;
            }   
        }
    }
}

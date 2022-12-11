using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAnimationScript : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [Header("Mimi Hit Box")]
    public CapacitiesHitBox laserHitBox;
    public bool shaking;
    public float shakingForce;
    public float shakingTime;
    public float shakingDuration;
    public Vector3 truePos;
    public float previousShake = 1;
    public float nextShakeTimer = 0.02f;
    public float shakeFrequency = 0.02f;

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

    public void Shake(float force, float time)
    {
        shakingForce = force;
        shakingTime = time;
        shakingDuration = time;
        shaking = true;
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

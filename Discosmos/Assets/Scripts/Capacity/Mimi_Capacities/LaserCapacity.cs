using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class LaserCapacity : ActiveCapacity
{
    public LaserCapacitySO laserSO;

    public override void InitializeCapacity(ActiveCapacitySO so)
    {
        base.InitializeCapacity(so);
        laserSO = (LaserCapacitySO)so;
    }

    public override void Cast()
    {
        if(onCooldown)
        {
            Debug.Log("Capacity on cooldown");
            return;
        }

        serverTimeBackup = PhotonNetwork.Time;
        GameAdministrator.OnServerUpdate += CastRoutine;
        Debug.Log("Start cast the mimi laser at " + PhotonNetwork.Time);
    }

    public override void CastRoutine()
    {
        if (castTimer >= laserSO.castTime)
        {
            Active();
            GameAdministrator.OnServerUpdate -= CastRoutine;
            castTimer = 0;
        }
        else
        {
            castTimer = (float)(PhotonNetwork.Time - serverTimeBackup);
        }
    }

    public override void Active()
    {
        owner.OnCastEnd?.Invoke(Capacities.MIMI_Laser);
        onCooldown = true;
        GameAdministrator.OnServerUpdate += Cooldown;
    }

    public override int[] GetTargets(Vector3 initPos)
    {
        
        Vector3 rectCenter = new Vector3(initPos.x, 1, laserSO.range/2);
        Vector3 halfExtend = new Vector3(laserSO.size, 0.3f / 2, laserSO.size);

        Collider[] hit = Physics.OverlapBox(rectCenter, halfExtend / 2);

        List<int> hitID = new List<int>();
        
        foreach (var targets in hit)
        {
            var targetView = targets.GetComponent<PhotonView>();
            if (targetView != null)
            {
                hitID.Add(targetView.ViewID);
            }
        }

        return hitID.Count switch
        {
            <= 0 => hitID.ToArray(),
            _ => base.GetTargets(initPos)
        };
    }

    public override void Cooldown()
    {
        if (cooldownTimer >= laserSO.cooldownTime)
        {
            Debug.Log("Capacity is no longer on cooldown");
            onCooldown = false;
            GameAdministrator.OnServerUpdate -= Cooldown;
            cooldownTimer = 0;
        }
        else
        {
            cooldownTimer = (float)(PhotonNetwork.Time - serverTimeBackup);
        }
        
    }
}

using System.Collections.Generic;
using System.Numerics;
using Photon.Pun;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class LaserCapacity : ActiveCapacity
{
    private LaserCapacitySO laserSO;

    public override void InitializeCapacity()
    {
        SetCapacityData(laserSO);
    }

    public override void Cast()
    {
        if(onCooldown) return;

        serverTimeBackup = PhotonNetwork.Time;
        GameAdministrator.OnServerUpdate += CastRoutine;
        Debug.Log(laserSO.name + " Start cast");
    }

    public override void CastRoutine()
    {
        if (castTimer >= laserSO.castTime)
        {
            Active();
            GameAdministrator.OnServerUpdate -= CastRoutine;
        }
        else
        {
            castTimer = (float)(PhotonNetwork.Time - serverTimeBackup);
        }
    }

    public override void Active()
    {
        Debug.Log(laserSO.name + " Active");
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
        
        
    }
}

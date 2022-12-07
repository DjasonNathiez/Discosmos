using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fow : MonoBehaviour
{
    public Transform myTarget;
    [SerializeField] bool isStatic = false;


    public FieldOfView myFieldOfView;

    [SerializeField] float followSpeed = 10;
    [SerializeField] float fowRadius = 10;

    PlayerModule playerModule;

    AnimationCurve curveInBrume;

    public void Start()
    {
        Init(myTarget, fowRadius);
    }

    public void Init(Transform _target = null, float _fowRadius = 0)
    {
        if(_target != null)
        {
            isStatic = false;
            myTarget = _target;
            fowRadius = _fowRadius;
            myFieldOfView.viewRadius = fowRadius;
        }
        else
        {
            isStatic = true;
            myFieldOfView.GenerateFowStatic();
        }
    }

    //public void InitPlayerModule(PlayerModule _pModule)
    //{
    //    playerModule = _pModule;
    //
    //    curveInBrume = new AnimationCurve();
    //    curveInBrume.AddKey(new Keyframe(0, playerModule.characterParameters.minVisionRangeInBrume));
    //    curveInBrume.AddKey(new Keyframe(1f, playerModule.characterParameters.minVisionRangeInBrume));
    //    curveInBrume.AddKey(new Keyframe(2f, playerModule.characterParameters.visionRangeInBrume));
    //}

    // Update is called once per frame
    void Update()
    {
        if (isStatic) { return; }

        transform.position = Vector3.Lerp(transform.position, new Vector3(myTarget.position.x, 0, myTarget.position.z), Time.deltaTime * followSpeed);

        float radius = 0;
        //if (playerModule.isInBrume)
        //{
        //    radius = curveInBrume.Evaluate(playerModule.timeInBrume);
        //}
        //else
        //{
        //    radius = playerModule.characterParameters.visionRange;
        //}

        //myFieldOfView.viewRadius = fowRadius;
    }
}

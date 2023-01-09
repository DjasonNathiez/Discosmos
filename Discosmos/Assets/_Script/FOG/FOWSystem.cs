using System;
using UnityEngine;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Serialization;

public class FOWSystem : MonoBehaviour
{
    private static FOWSystem _instance;
    public static FOWSystem Instance { get { return _instance; } }

    public Color fogColor = new Color(0.05f, 0.05f, 0.05f, 1f);
    public Color bushColor = new Color(0.05f, 0.05f, 0.05f, 1f);
    
    [HideInInspector]
    public Color currentFogColor = new Color(0.05f, 0.05f, 0.05f, 1f);

    public Material maskMatPlane;
    public Material fogMat;

    public int worldSize = 256;

    public float blendFactor = 0;

    public AnimationCurve opacityCurve;
    
    //Debug
    public  List<PlayerModule> localPlayer;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        //maskMatPlane.SetFloat("_Opacity", 0);
    }

    float opacityValue = 0;
    //private void Update()
    //{
    //    foreach (PlayerModule localPlayer in localPlayer)
    //    {
    //        if (localPlayer != null)
    //        {
    //            if (localPlayer.isInBrush)
    //            {
    //                currentFogColor = Color.Lerp(currentFogColor, bushColor, Time.deltaTime * 5);
    //                opacityValue = opacityCurve.Evaluate(localPlayer.timeInBrume);
    //            }
    //            else
    //            {
    //                currentFogColor = Color.Lerp(currentFogColor, fogColor, Time.deltaTime * 5);
    //                opacityValue = Mathf.Lerp(opacityValue, 0, Time.deltaTime * 5);
    //            }
    //        
    //            //maskMatPlane.SetFloat("_Opacity", opacityValue);
    //        }
    //    }
    //}
    //
    private void OnDisable()
    {
        //maskMatPlane.SetFloat("_Opacity", 0);
        fogMat.SetColor("_Unexplored", Color.black);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(worldSize *0.9f, 10, worldSize *0.9f));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(worldSize , 10, worldSize));
    }
}

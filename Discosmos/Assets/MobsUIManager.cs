using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class MobsUIManager : MonoBehaviour
{
    public static MobsUIManager instance;
    public Transform canvas;

    private void Awake()
    {
        instance = this;
    }
}

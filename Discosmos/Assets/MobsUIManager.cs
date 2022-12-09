using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class MobsUIManager : MonoBehaviour
{
    public List<MinionsController> mobs;
    public GameObject healthBar;
    [SerializeField] public List<HealthBar> healthBars;
    public static MobsUIManager instance;
    public Transform canvas;
    public Camera camPlayer;
    public float heightUI;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        camPlayer = GameAdministrator.instance.localPlayer.GetComponent<PlayerManager>()._camera;
    }

    public void MinionSpawned(MinionsController controller)
    {
        mobs.Add(controller);
        controller.id = mobs.Count - 1;
        HealthBar bar = new HealthBar();
        bar.transform = Instantiate(healthBar, Vector3.zero, quaternion.identity, canvas).transform;
        bar.healthFill = bar.transform.GetChild(0).GetComponent<Image>();
        bar.healthText = bar.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        bar.target = bar.transform.GetChild(2);
        healthBars.Add(bar);
    }

    private void Update()
    {
        for (int i = 0; i < healthBars.Count; i++)
        {
            healthBars[i].transform.position = camPlayer.WorldToScreenPoint(mobs[i].transform.position + Vector3.up) + Vector3.up * heightUI;
            healthBars[i].healthFill.fillAmount = mobs[i].currentHealth / (float) mobs[i].maxHealth;
            healthBars[i].healthText.text = mobs[i].currentHealth + " / " + mobs[i].maxHealth;
        }
    }
}


[Serializable]
public class HealthBar
{
    public Transform transform;
    public Image healthFill;
    public Image speedFill;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI nameText;
    public Transform target;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveInfo : MonoBehaviour
{
    public static SaveInfo instance;
    private void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [HideInInspector]
    public Vector3 lastSavePosition;
    [HideInInspector]
    public float lastSaveHealth;
    [HideInInspector]
    public bool hasFireKey;
    [HideInInspector]
    public bool hasIceKey;

    public void SetSaveInfo()
    {
        PlayerCharControl playerInstance = PlayerCharControl.instance;
       
        lastSavePosition = playerInstance.transform.position;
        lastSaveHealth = playerInstance.GetHealth();

        hasFireKey = playerInstance.GetFireKey();
        hasIceKey = playerInstance.GetIceKey();
    }
}

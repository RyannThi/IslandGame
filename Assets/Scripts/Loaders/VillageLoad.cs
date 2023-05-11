using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageLoad : MonoBehaviour
{
    public string villageToLoadName;
    [SerializeField]
    private Collider loadRadius;

    private void Start()
    {
        loadRadius = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadSceneAsync(villageToLoadName,LoadSceneMode.Additive);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.UnloadSceneAsync(villageToLoadName);
        }

    }

}

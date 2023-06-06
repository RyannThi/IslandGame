using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageLoad : MonoBehaviour
{
    public string villageToLoadName;
    private Collider loadRadius;

    [SerializeField]
    private GameObject lodVillage;

    private void Start()
    {
        loadRadius = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lodVillage.SetActive(false);
            SceneManager.LoadSceneAsync(villageToLoadName,LoadSceneMode.Additive);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lodVillage.SetActive(true);
            SceneManager.UnloadSceneAsync(villageToLoadName);
        }

    }

}
